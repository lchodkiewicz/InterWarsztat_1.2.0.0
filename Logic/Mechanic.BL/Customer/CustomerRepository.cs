using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnzoMiniCrm.Model;
using log4net;

namespace Mechanik.Repository
{
    public partial class CustomerRepository
    {
        protected static readonly ILog _log = LogManager.GetLogger("CustomerRepository");
        private Database _db;

        public CustomerRepository(Database db)
        {
            _db = db;
        }
        public string Save(Customer entityCustomer, bool isNew)
        {
            StringBuilder result = new StringBuilder();
            //TODO Validation???

            try
            {
                if (isNew)
                {
                    _db.Insert(entityCustomer);
                }
                else
                {
                    _db.Save(entityCustomer);
                }

            }
            catch (Exception ex)
            {
                result.Append(ex.Message);
                _log.Error(ex.Message);

            }
            return result.ToString();
        }
        public Customer GetById(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                var entity = _db.SingleOrDefault<Customer>((new Sql("SELECT t.* FROM [dbo].[Customers] t WHERE t.CustomerID = @0", id)));
                return entity;
            }
            else
            {
                return new Customer();
            }
        }

        }
    }

