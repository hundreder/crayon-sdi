using Crayon.Domain.Errors;
using Crayon.Domain.Models;
using Crayon.Repository;
using Crayon.Repository.ApiClients;
using Crayon.Services.Common;
using Crayon.Services.Models;
using Crayon.Services.Services;
using Crayon.Services.Services.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Crayon.Tests.Services;

[TestFixture]
public class OrdersServiceTests
{
    private CrayonDbContext _dbContext;
    private Mock<ISoftwareCatalogService> _softwareCatalogServiceMock;
    private Mock<IMediator> _mediatorMock;
    private Mock<ICcpApiClient> _ccpApiClientMock;
    private IDateTimeProvider _dateTimeProvider = new DateTimeProvider();
    private OrdersService _ordersService;
    private NewOrder _newOrder;
    private CancellationToken cancellationToken = CancellationToken.None;
    private Account _account = new Account { Id = 1, CustomerId = 1, Name = "Foo Account" };

    [SetUp]
    public void Setup()
    {
        // Setup In-Memory DbContext
        var dbOptions = new DbContextOptionsBuilder<CrayonDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new CrayonDbContext(dbOptions);

        // Mock other dependencies
        _softwareCatalogServiceMock = new Mock<ISoftwareCatalogService>();
        _softwareCatalogServiceMock.Setup(service =>
                service.GetSoftware(It.IsAny<List<int>>(), cancellationToken))
            .ReturnsAsync(new List<Software> { new Software(100, "Wrd", "1.0", "MS", 100m) }); // Software is available

        _mediatorMock = new Mock<IMediator>();
        _mediatorMock.Setup(mediator =>
                mediator.Publish(It.IsAny<CompletedOrderEvent>(), cancellationToken))
            .Returns(Task.CompletedTask);
        
        _ccpApiClientMock = new Mock<ICcpApiClient>();

        //new order setup for most cases
        _newOrder = new NewOrder(
            AccountId: 1,
            CustomerId: 1,
            Items: new List<NewOrderItem>
            {
                new NewOrderItem(SoftwareId: 100, LicenseCount: 1, LicencedExpiration: DateTime.UtcNow.AddYears(1))
            });

        
        _ordersService = new OrdersService(
            _dbContext,
            _softwareCatalogServiceMock.Object,
            _mediatorMock.Object,
            _dateTimeProvider,
            _ccpApiClientMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Test]
    public async Task CreateOrder_EmptyItems_ReturnsNoItemsInOrder()
    {
        // Arrange
        var epmtyNewOrder = new NewOrder(
            AccountId: 1, 
            CustomerId: 1, 
            Items: new List<NewOrderItem>()); // No items
        
        // Act
        var result = await _ordersService.CreateOrder(epmtyNewOrder, cancellationToken);

        // Assert
        Assert.That(result.IsLeft);
        Assert.That(CreateOrderError.NoItemsInOrder, Is.EqualTo(result.LeftAsEnumerable().First()));
    }

    [Test]
    public async Task CreateOrder_AccountNotBelongingToUser_ReturnsAccountNotFound()
    {
        // Arranged already

        // Add account in the db context not belonging to the user
        await _dbContext.Accounts.AddAsync(new Account()
        {
            Id = 1,
            CustomerId = 2,
            Name = "Foo Account"
        });
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _ordersService.CreateOrder(_newOrder, cancellationToken);

        // Assert
        Assert.That(result.IsLeft);
        Assert.That(CreateOrderError.AccountNotFound, Is.EqualTo(result.LeftAsEnumerable().First()));
    }

    [Test]
    public async Task CreateOrder_SoftwareNotAvailable_ReturnsSoftwareNotFound()
    {
        // Arrange

        // Add account belonging to user
        await _dbContext.Accounts.AddAsync(_account);
        await _dbContext.SaveChangesAsync();
        
        // Mock software catalog service to return no software available
        _softwareCatalogServiceMock.Setup(service =>
                service.GetSoftware(It.IsAny<List<int>>(), cancellationToken))
            .ReturnsAsync([]); // No software available
        
        // Act
        var result = await _ordersService.CreateOrder(_newOrder, cancellationToken);

        // Assert
        Assert.That(result.IsLeft);
        Assert.That(CreateOrderError.SoftwareNotFound, Is.EqualTo(result.LeftAsEnumerable().First()));
    }

    [Test]
    public async Task CreateOrder_ValidOrder_CreatesOrderSuccessfully()
    {
        // Arrange

        // Add account belonging to user
        await _dbContext.Accounts.AddAsync(_account);
        await _dbContext.SaveChangesAsync();
        
        // Mock external API call (SendOrder)
        var ccpOrderId = Guid.NewGuid().ToString();
        _ccpApiClientMock.Setup(client =>
                client.SendOrder(It.IsAny<Order>()))
            .ReturnsAsync(new CcpOrder(ccpOrderId, new List<SoftwareLicence>()
            {
                new(100, Guid.NewGuid().ToString(), 1, DateTime.UtcNow.AddYears(1))
            }));
       

        // Act
        var result = await _ordersService.CreateOrder(_newOrder, cancellationToken);

        // Assert
        Assert.That(result.IsRight);
        var completedOrder = result.RightAsEnumerable().First();
        Assert.That(completedOrder != null, Is.True);
        Assert.That(ccpOrderId, Is.EqualTo(completedOrder.CcpOrderId)); // Verify correct mapping
    }

    
    [Test]
    public async Task CreateOrder_ExternalApiFailure_SavesOrderAsFailed()
    {
        // Arrange
    
        // Add account belonging to user
        await _dbContext.Accounts.AddAsync(_account);
        await _dbContext.SaveChangesAsync();
    
        
        // Mock external API to return a failure
        _ccpApiClientMock.Setup(client =>
                client.SendOrder(It.IsAny<Order>()))
            .ReturnsAsync(CreateOrderError.SubmittingOrderToExternalProviderFailed);
    
        // Act
        var result = await _ordersService.CreateOrder(_newOrder, cancellationToken);
    
        // Assert
        Assert.That(result.IsLeft);
        Assert.That(OrderStatus.Failed,Is.EqualTo( _dbContext.Orders.First().Status)); // Verify order saved as Failed
    }
}