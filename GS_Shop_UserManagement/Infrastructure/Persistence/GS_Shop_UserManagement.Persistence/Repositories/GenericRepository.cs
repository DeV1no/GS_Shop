﻿using GS_Shop_UserManagement.Application.Contracts.Persistence;
using GS_Shop_UserManagement.Infrastructure.Logging.Mongo.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GS_Shop_UserManagement.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly GSShopUserManagementDbContext _context;
    // private readonly IMongoLoggerService _mongoLoggerService;
    //
    // public GenericRepository(GSShopUserManagementDbContext context, MongoLoggerService mongoLoggerService)
    // {
    //     _context = context;
    //     _mongoLoggerService = mongoLoggerService;
    // }
    public GenericRepository(GSShopUserManagementDbContext context)
    {
        _context = context;
    }


    public async Task<T?> Get(int id)
    {
        var dd = await _context.Set<T>().FindAsync(id);
        return dd;
    }

    // => await _context.Set<T>().FindAsync(id);

    public async Task<IReadOnlyList<T>> GetAll() => await _context.Set<T>().ToListAsync();

    public async Task<T> Add(T entity)
    {
        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<T> Update(T entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> Exist(int id)
    {
        var entity = await Get(id);
        return entity != null;
    }

}