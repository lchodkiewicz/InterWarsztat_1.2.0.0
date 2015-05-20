namespace EnzoMiniCrm.Model
{
using EnzoMiniCrm.Model;
using log4net;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

    /// <summary>
    /// This class describes the data layer related to Mail.
    /// </summary>
    /// <history>
    ///   <change author=`Auto Generated` date=27-04-2015>Original Version</change>
    /// </history>
    public partial class MailRepository
    {
	 protected static readonly ILog _log = LogManager.GetLogger("MailRepository");

        private Database _db;
        public MailRepository(Database db)
        {
            _db = db;
        }
        public string Save(Mail entityMail, bool isNew)
        {
            StringBuilder result = new StringBuilder();
            //TODO Validation???

            try
            {
                if (isNew)
                {
                    _db.Insert(entityMail);
                }
                else
                {
                    _db.Save(entityMail);
                }

            }
            catch (Exception ex)
            {
                result.Append(ex.Message);
                _log.Error(ex.Message);

            }
            return result.ToString();
        }
        //public string Delete(Mail entity)
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

        public Mail GetById(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                var entity = _db.SingleOrDefault<Mail>((new Sql("SELECT t.* FROM [dbo].[Mails] t WHERE t.MailID = @0", id)));
                return entity;
            }
            else
            {
                return new Mail();
            }
        }

	}
}