using System.Reflection;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using Vayosoft.AutoMapper;
using Vayosoft.Core;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.Persistence.Commands;
using Vayosoft.Core.Persistence.Queries;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Data.EF.MySQL;
using Vayosoft.Data.MongoDB;
using Vayosoft.Data.MongoDB.QueryHandlers;
using Warehouse.Core.Entities.Events;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Entities.Models.Payloads;
using Warehouse.Core.Entities.ValueObjects;
using Warehouse.Core.Persistence;
using Warehouse.Core.Services.Providers;
using Warehouse.Core.UseCases;
using Warehouse.Core.UseCases.Administration.Spcecifications;
using Warehouse.Core.UseCases.IPS;
using Warehouse.Core.UseCases.IPS.Models;
using Warehouse.Core.UseCases.IPS.Queries;
using Warehouse.Core.UseCases.IPS.Specifications;
using Warehouse.Core.UseCases.OperationHistory;
using Warehouse.Core.UseCases.Products.Commands;
using Warehouse.Core.UseCases.Products.Handlers;
using Warehouse.Core.UseCases.Products.Queries;
using Warehouse.Core.UseCases.Products.Specifications;
using Warehouse.Core.UseCases.Providers;
using Warehouse.Core.UseCases.Warehouse;
using Warehouse.Core.UseCases.Warehouse.Commands;
using Warehouse.Core.UseCases.Warehouse.Models;
using Warehouse.Core.UseCases.Warehouse.Queries;
using Warehouse.Core.UseCases.Warehouse.Specifications;

namespace Warehouse.Core
{
    public static class Config
    {
        public static IServiceCollection AddWarehouseDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCoreServices();

            var domainAssembly = AppDomain.CurrentDomain.GetAssemblies();
            ConventionalProfile.Scan(domainAssembly);

            services.AddSingleton(provider =>
            {
                var mapperConfiguration = new MapperConfiguration(cfg =>
                {
                    ConventionalProfile.Scan(domainAssembly);
                    cfg.AddProfile<ConventionalProfile>();
                    cfg.AddProfile<MappingProfile>();
                });
                return new AutoMapperWrapper(mapperConfiguration);
            });
            services.AddSingleton(typeof(IProjector), provider => provider.GetRequiredService<AutoMapperWrapper>());
            services.AddSingleton(typeof(Vayosoft.Core.SharedKernel.IMapper), provider => provider.GetRequiredService<AutoMapperWrapper>());

            services
                .AddEntityDependencies(configuration)
                .AddMongodDependencies(configuration);

            services.AddDefaultProvider()
                .AddProviderHandlers();
            
            services.AddOperationHistory();

            return services;
        }

        public static void AddOperationHistory(this IServiceCollection services)
        {
            services.AddScoped<IRepository<OperationHistoryEntity, string>, WarehouseRepository<OperationHistoryEntity>>();
            services.AddScoped<INotificationHandler<OperationOccurred>, OperationEventHandler>();
        }

        public static IServiceCollection AddMongodDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMongoDbContext(ConfigureMongoDb);

            //repositories
            services.AddScoped<IRepository<FileEntity, string>, WarehouseRepository<FileEntity>>();
            services.AddScoped<IRepository<ProductEntity, string>, WarehouseRepository<ProductEntity>>();
            services.AddScoped<IRepository<WarehouseSiteEntity, string>, WarehouseRepository<WarehouseSiteEntity>>();
            services.AddScoped<IReadOnlyRepository<ProductEntity>, WarehouseRepository<ProductEntity>>();
            services.AddScoped<IRepository<BeaconEntity, string>, WarehouseRepository<BeaconEntity>>();
            services.AddScoped<IReadOnlyRepository<BeaconRegisteredEntity>, WarehouseRepository<BeaconRegisteredEntity>>();
            services.AddScoped<IRepository<IndoorPositionStatusEntity, string>, WarehouseRepository<IndoorPositionStatusEntity>>();

            //queries
            services.AddScoped<IRequestHandler<SetProduct, Unit>, ProductCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteProduct, Unit>, ProductCommandHandler>();
            services.AddScoped<IRequestHandler<GetProductMetadata, ProductMetadata>, ProductQueryHandler>();
            services.AddScoped<IRequestHandler<GetProductItemMetadata, ProductMetadata>, ProductQueryHandler>();
            services.AddScoped<IRequestHandler<GetAssets, IPagedEnumerable<AssetDto>>, AssetsQueryHandler>();
            services.AddScoped<IRequestHandler<GetAssetInfo, IEnumerable<AssetInfo>>, AssetsQueryHandler>();
            services.AddScoped<IRequestHandler<GetBeaconTelemetry, BeaconTelemetryDto>, AssetsQueryHandler>();
            services.AddScoped<IRequestHandler<GetBeaconTelemetry2, BeaconTelemetry2Dto>, AssetsQueryHandler>();
            services.AddScoped<IRequestHandler<GetIpsStatus, IndoorPositionStatusDto>, AssetsQueryHandler>();
            services.AddScoped<IRequestHandler<GetSiteInfo, IEnumerable<IndoorPositionStatusDto>>, AssetsQueryHandler>();
            services.AddScoped<IRequestHandler<GetRegisteredBeaconList, IEnumerable<string>>, WarehouseQueryHandler>();
            services.AddScoped<IRequestHandler<GetProductItems, IPagedEnumerable<ProductItemDto>>, WarehouseQueryHandler>();
            services.AddScoped<IRequestHandler<GetRegisteredGwList, IEnumerable<string>>, GetRegisteredGwList.RegisteredGwQueryHandler>();

            services.AddScoped<IRequestHandler<SpecificationQuery<WarehouseSiteSpec, IPagedEnumerable<WarehouseSiteEntity>>, IPagedEnumerable<WarehouseSiteEntity>>,
                MongoPagingQueryHandler<WarehouseSiteSpec, WarehouseSiteEntity>>();
            services.AddScoped<IRequestHandler<SpecificationQuery<WarehouseProductSpec, IPagedEnumerable<BeaconRegisteredEntity>>, IPagedEnumerable<BeaconRegisteredEntity>>,
                MongoPagingQueryHandler<WarehouseProductSpec, BeaconRegisteredEntity>>();
            services.AddScoped<IRequestHandler<SpecificationQuery<BeaconEventSpec, IPagedEnumerable<BeaconEventEntity>>, IPagedEnumerable<BeaconEventEntity>>,
                MongoPagingQueryHandler<BeaconEventSpec, BeaconEventEntity>>();
            services.AddScoped<IRequestHandler<SpecificationQuery<BeaconPositionSpec, IPagedEnumerable<BeaconIndoorPositionEntity>>, IPagedEnumerable<BeaconIndoorPositionEntity>>,
                MongoPagingQueryHandler<BeaconPositionSpec, BeaconIndoorPositionEntity>>();
            services.AddScoped<IRequestHandler<SpecificationQuery<ProductSpec, IPagedEnumerable<ProductEntity>>, IPagedEnumerable<ProductEntity>>,
                MongoPagingQueryHandler<ProductSpec, ProductEntity>>();
            services.AddScoped<IRequestHandler<SingleQuery<ProductEntity>, ProductEntity>, MongoSingleQueryHandler<string, ProductEntity>>();

            //commands
            services.AddScoped<IRequestHandler<SetWarehouseSite, Unit>, WarehouseCommandHandler>();
            services.AddScoped<IRequestHandler<DeleteWarehouseSite, Unit>, WarehouseCommandHandler>();
            services.AddScoped<IRequestHandler<SetGatewayToSite, Unit>, WarehouseCommandHandler>();
            services.AddScoped<IRequestHandler<RemoveGatewayFromSite, Unit>, WarehouseCommandHandler>();
            services.AddScoped<IRequestHandler<SetBeacon, Unit>, WarehouseCommandHandler>();

            return services;
        }

        public static IServiceCollection AddEntityDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMySqlContext(configuration);
            services.AddScoped<IUnitOfWork>(s => s.GetRequiredService<DataContext>());
            services.AddScoped<ILinqProvider>(s => s.GetRequiredService<DataContext>());

            services.AddScoped<IRequestHandler<SpecificationQuery<UserSpec, IPagedEnumerable<UserEntityDto>>, IPagedEnumerable<UserEntityDto>>,
                PagingQueryHandler<long, UserSpec, UserEntity, UserEntityDto>>();
            services
                .AddScoped<IRequestHandler<SingleQuery<UserEntityDto>, UserEntityDto>,
                    SingleQueryHandler<long, UserEntity, UserEntityDto>>();

            services.AddScoped<IRequestHandler<CreateOrUpdateCommand<UserEntityDto>, Unit>, CreateOrUpdateHandler<long, UserEntity, UserEntityDto>>();

            return services;
        }

        public static void ConfigureMongoDb()
        {
            AutoRegistration.RegisterClassMap(Assembly.GetExecutingAssembly());
            BsonSerializer.RegisterSerializer(typeof(MacAddress), new MacAddressSerializer());
            //db.setProfilingLevel(2,1)
        }

    }

    public class MacAddressSerializer : IBsonSerializer<MacAddress>
    {
        object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return Deserialize(context, args);
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, MacAddress value)
        {
            context.Writer.WriteString(value.Value);
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            if (value is MacAddress macAddress)
            {
                context.Writer.WriteString(macAddress.Value);
            }
            else
            {
                throw new NotSupportedException("This is not an MacAddress");
            }
        }

        public MacAddress Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var value = context.Reader.ReadString();
            return MacAddress.Create(value);
        }

        public Type ValueType => typeof(MacAddress);
    }

    //public class MacAddressConverter : JsonConverter<MacAddress>
    //{
    //    public override void WriteJson(JsonWriter writer, MacAddress value, JsonSerializer serializer)
    //    {
    //        writer.WriteValue(value.Value);
    //    }

    //    public override MacAddress ReadJson(JsonReader reader, Type objectType, MacAddress existingValue, bool hasExistingValue,
    //        JsonSerializer serializer)
    //    {
    //        if (reader.TokenType == JsonToken.String)
    //        {
    //            //if (reader.Value != null)
    //            return MacAddress.Create((reader.Value as string)!);
    //        }
    //        else
    //        {
    //            throw new NotSupportedException("This is not an MacAddress value");
    //        }
    //    }
    //}

    //public class MacAddressConverter : JsonConverter<MacAddress>
    //{
    //    public override MacAddress? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    //    {
    //        if (reader.TokenType == JsonTokenType.String)
    //        {
    //            return MacAddress.Create(reader.GetString()!);
    //        }
    //        else
    //        {
    //            throw new NotSupportedException("This is not an MacAddress value");
    //        }
    //    }

    //    public override void Write(Utf8JsonWriter writer, MacAddress value, JsonSerializerOptions options)
    //    {
    //        writer.WriteStringValue(value.ToString());
    //    }
    //}
}
