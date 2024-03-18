using ESG_Survey_Automation.Infrastructure.EntityFramework;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESG_Survey_Automation.UnitTests
{
    public class ESGSurveyContextTestBase
    {
        protected DbContextOptions<ESGSurveyContext> _options;
        protected ESGSurveyContext _context;
        public ESGSurveyContextTestBase()
        {
            _options = new DbContextOptionsBuilder<ESGSurveyContext>().UseSqlite("DataSource=:memory:").Options;
            _context = new ESGSurveyContext(_options);
            _context.Database.OpenConnection();
            var con = _context.Database.GetDbConnection() as SqliteConnection;
            _context.Database.EnsureCreated();
        }

        protected TEntity[] AddEntityToDatabase<TEntity>(params TEntity[] entities)
        {
            foreach (var entity in entities)
            {
                if(entity != null)
                {
                    _context.Add(entity);
                }                
            }
            _context.SaveChanges();
            foreach (var entity in entities)
            {
                if (entity != null)
                {
                    _context.Entry(entity).State = EntityState.Modified;
                }
            }
            return entities;
        }
    }
}
