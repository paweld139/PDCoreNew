﻿using CsvHelper.Configuration;

namespace PDCoreTest
{
    class CustomerMap : ClassMap<Customer>
    {
        public CustomerMap()
        {
            //Map(m => m.Property1).Name("Column Name");
            //Map(m => m.Property2).Index(4);
            //Map(m => m.Property3).Ignore();
            //Map(m => m.Property4).TypeConverter<MySpecialTypeConverter>();

            Map(x => x.Id).Index(0);
            Map(x => x.Name).Index(1);
            Map(x => x.Address).Index(2);
            Map(x => x.NIP).Index(3);
            Map(x => x.PhoneNumber).Index(4);
        }
    }
}
