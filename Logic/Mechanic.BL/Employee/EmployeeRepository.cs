using EnzoMiniCrm.Model;
using log4net;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mechanic.BL.Models
{
    class EmployeeRepository
    {
    protected static readonly ILog _log = LogManager.GetLogger("EmployeeRepository");

        private Database _db;
        public EmployeeRepository(Database db)
        {
            _db = db;
        }
        public string Save(Employee entityEmployee, bool isNew)
        {
            StringBuilder result = new StringBuilder();
            //TODO Validation???

            try
            {
                if (isNew)
                {
                    _db.Insert(entityEmployee);
                }
                else
                {
                    _db.Save(entityEmployee);
                }

            }
            catch (Exception ex)
            {
                result.Append(ex.Message);
                _log.Error(ex.Message);

            }
            return result.ToString();
        }
        //public string Delete(Employee entity)
        //{
        //    var finalResult = "";
        //    //ValidationResult results = validator.Validate(entity);
        //    if (!results.IsValid)
        //    {
        //        finalResult = results.Errors.Select(i => i.ErrorMessage).ToString(",");
        //    }
        //    else
        //    {
        //        _db.Delete(entity);
        //    }
        //    return finalResult;
        //}

        public Employee GetById(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                var entity = _db.SingleOrDefault<Employee>((new Sql("SELECT t.* FROM [dbo].[Employees] t WHERE t.EmployeeID = @0", id)));
                return entity;
            }
            else
            {
                return new Employee();
            }
        }

        }
    }

