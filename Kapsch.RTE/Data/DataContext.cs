using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Kapsch.RTE.Data
{
    public class DataContext : DbContext
    {
        /// <summary>
        ///     Constructor
        /// </summary>

        public DataContext()
        {
            Configuration.LazyLoadingEnabled = true;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // http://stackoverflow.com/questions/7924758/entity-framework-creates-a-plural-table-name-but-the-view-expects-a-singular-ta
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            base.OnModelCreating(modelBuilder);
        }

        public IDbSet<ConfigurationDotSection> DotSectionConfigurations { get; set; }
        public IDbSet<CameraPointData> CameraPointsData { get; set; }}
}