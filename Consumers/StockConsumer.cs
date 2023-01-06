using MassTransit;
using Savana.Product.API.Entities;
using Treasures.Common.Events;
using Treasures.Common.Interfaces;

namespace Savana.Product.API.Consumers; 

public class StockConsumer : IConsumer<StockEvent> {
    private readonly IUnitOfWork _unitOfWork;

    public StockConsumer(IUnitOfWork unitOfWork) {
        _unitOfWork = unitOfWork;
    }
    public async Task Consume(ConsumeContext<StockEvent> context) {
        var message = context.Message;
        var existingProd = await _unitOfWork.Repository<ProductEntity>().GetByIdAsync(message.ProductId);
        if(existingProd == null) return;

        existingProd.Quantity = message.Stock;

        _unitOfWork.Repository<ProductEntity>().UpdateAsync(existingProd);
        await _unitOfWork.Complete();
    }
}