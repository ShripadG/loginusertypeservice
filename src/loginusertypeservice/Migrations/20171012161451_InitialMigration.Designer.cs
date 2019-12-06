using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using employeeservice.Models;

namespace employeeservice.Migrations
{
    [DbContext(typeof(EmployeesDbContext))]
    [Migration("20171012161451_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.3");

            modelBuilder.Entity("employeeservice.Models.Employee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();
                    b.Property<string>("_id");
                    b.Property<string>("_rev");
                    b.Property<string>("EmployeeType");
                    b.Property<string>("Status");
                    b.Property<string>("FormattedEmployeedId");
                    b.Property<string>("IBMEmailID");
                    b.Property<string>("BandCategoriesFormulaeBased");
                    b.Property<string>("OffboardDate");
                    b.Property<string>("OffboardReqRecdDate");
                    b.Property<string>("NationwideEmailID");
                    b.Property<string>("BandonJoiningNationwide");
                    b.Property<string>("OffboardingReqRecdDate");
                    b.Property<string>("PMPEndDate");
                    b.Property<string>("RowAddedDate");
                    b.Property<string>("WorkLocation");
                    b.Property<string>("IBMID");
                    b.Property<string>("Offboardingrequestdayscategories");
                    b.Property<string>("OnboardingReqRecdDate");
                    b.Property<string>("Remarks");
                    b.Property<string>("AccountOffboardingOffboardedDate");
                    b.Property<string>("RoleAtIBM");
                    b.Property<string>("CID");
                    b.Property<string>("objectId");
                    b.Property<string>("LOB");
                    b.Property<string>("Billable");
                    b.Property<string>("ContractService");
                    b.Property<string>("created");
                    b.Property<string>("AccountOnboardDate");
                    b.Property<string>("PortfolioLeadOffshore");
                    b.Property<string>("HCAMID");
                    b.Property<string>("EndDate");
                    b.Property<string>("SourceofInformation");
                    b.Property<string>("DaysBetween");
                    b.Property<string>("DGDCSquad");
                    b.Property<string>("FTE");
                    b.Property<string>("CountryFormulaeBased");
                    b.Property<string>("Country");
                    b.Property<string>("LocationStatus");
                    b.Property<string>("updated");
                    b.Property<string>("EmployeeName");
                    b.Property<string>("ReasonforLeaving");
                    b.Property<string>("Action");
                    b.Property<string>("LandedHCAMorIA");
                    b.Property<string>("OnboardingreqdaysSLAcategories");
                    b.Property<string>("CurrentWorkLocation");
                    b.Property<string>("TenureinMonths");
                    b.Property<string>("OnboardDate");
                    b.Property<string>("Gender");
                    b.Property<string>("OnboardingrequestdaysSLA");
                    b.Property<string>("TenureCategories");
                    b.Property<string>("ownerId");
                    b.Property<string>("UnformattedEmployeeId");
                    b.Property<string>("OnboardRequestReceivedDate");
                    b.Property<string>("CurrentBand");
                    b.Property<string>("DGDC");
                    b.Property<string>("OffboardingrequestdaysSLAcategories");

                    b.HasKey("Id");

                    b.ToTable("Employees");
                });
        }
    }
}
