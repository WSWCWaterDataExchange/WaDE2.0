﻿using AutoMapper;
using System;
using WesternStatesWater.WaDE.Managers.Api.Mapping;

namespace WesternStatesWater.WaDE.Managers.Mapping
{
    internal static class DtoMapper
    {
        static IMapper _mapper;
        private static IConfigurationProvider _config;

        static IMapper Mapper => _mapper ?? (_mapper = Configuration.CreateMapper());

        public static IConfigurationProvider Configuration
        {
            get
            {
                if (_config == null)
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.AddProfile<ApiProfile>();
                        cfg.AddProfile<OgcApiProfile>();
                    });
                    _config = config;
                }
                return _config;
            }
        }

        public static T Map<T>(this object source, Action<IMappingOperationOptions> opts = null)
        {
            return Mapper.Map<T>(source, opts ?? (a => { }));
        }

        public static void Map(this object source, object dest, Action<IMappingOperationOptions> opts = null)
        {
            Mapper.Map(source, dest, opts ?? (a => { }));
        }
    }
}
