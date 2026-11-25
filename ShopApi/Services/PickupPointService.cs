using Microsoft.EntityFrameworkCore;
using ShopApi.Models;
using ShopApi.DTO;

namespace ShopApi.Services
{
    public class PickupPointService
    {
        private readonly ISRPO_PR4Context _context;
        public PickupPointService(ISRPO_PR4Context context)
        {
            _context = context;
        }

        public async Task<List<PickupPointDtos.PickupPointResponse>> GetAllPickupPointsAsync()
        {
            return await _context.PickupPoints.Select(p => new PickupPointDtos.PickupPointResponse(p.Id, p.Address, p.Description)).ToListAsync();
        }

        public async Task<(PickupPointDtos.PickupPointResponse?, string? Error)> CreateAsync(PickupPointDtos.PickupPointRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Address)) return (null, "Адрес пункта выдачи обязателен");

            var exists = await _context.PickupPoints.AnyAsync(p => p.Address == request.Address);
            if (exists) return (null, "Пункт выдачи с таким адресом уже существует");

            var pickupPoint = new PickupPoints
            {
                Address = request.Address,
                Description = request.Description
            };
            _context.PickupPoints.Add(pickupPoint);
            await _context.SaveChangesAsync();
            return (new PickupPointDtos.PickupPointResponse(pickupPoint.Id, pickupPoint.Address, pickupPoint.Description), null);
        }

        public async Task<(PickupPointDtos.PickupPointResponse?, string? Error)> UpdateAsync(int id, PickupPointDtos.PickupPointRequest request)
        {
            var pickupPoint = await _context.PickupPoints.FindAsync(id);
            if (pickupPoint == null) return (null, "Пункт выдачи не найден");

            if (string.IsNullOrWhiteSpace(request.Address)) return (null, "Адрес пункта выдачи обязателен");

            var exists = await _context.PickupPoints.AnyAsync(p => p.Address == request.Address && p.Id != id);
            if (exists) return (null, "Пункт выдачи с таким адресом уже существует");

            pickupPoint.Address = request.Address;
            pickupPoint.Description = request.Description;
            await _context.SaveChangesAsync();
            return (new PickupPointDtos.PickupPointResponse(pickupPoint.Id, pickupPoint.Address, pickupPoint.Description), null);
        }

        public async Task<(PickupPointDtos.PickupPointResponse?, string? Error)> DeleteAsync(int id)
        {
            var pickupPoint = await _context.PickupPoints.FindAsync(id);
            if (pickupPoint == null) return (null, "Пункт выдачи не найден");

            var hasOrders = await _context.Orders.AnyAsync(o => o.PickupPointId == id);
            if (hasOrders) return (null, "Невозможно удалить пункт выдачи, так как он используется в заказах");

            _context.PickupPoints.Remove(pickupPoint);
            await _context.SaveChangesAsync();
            return (new PickupPointDtos.PickupPointResponse(pickupPoint.Id, pickupPoint.Address, pickupPoint.Description), null);
        }
    }
}
