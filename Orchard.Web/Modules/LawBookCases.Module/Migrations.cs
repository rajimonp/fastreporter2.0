using LawBookCases.Module.Models;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using System;

namespace LawBookCases.Module{
    public class Migrations : DataMigrationImpl {

        public int Create() {
            SchemaBuilder.CreateTable(typeof(CasePartArchiveRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("Year")
                    .Column<int>("Month")
                    .Column<int>("PostCount")
                    .Column<int>("CasePart_id")
                );

            SchemaBuilder.CreateTable(typeof(RecentCasePostsPartRecord).Name,
                table => table
                    .ContentPartRecord()
                    .Column<int>("CaseNumber")
                    .Column<int>("Count")
                );

            SchemaBuilder.CreateTable(typeof(CaseArchivesPartRecord).Name,
                table => table
                    .ContentPartRecord()
                    .Column<int>("CaseNumber")
                );

            ContentDefinitionManager.AlterPartDefinition(typeof(CasePart).Name, builder => builder
                .WithDescription("Turns content types into a Casse."));

            ContentDefinitionManager.AlterTypeDefinition("Case",
                cfg => cfg
                    .WithPart(typeof(CasePart).Name)
                    .WithPart("CommonPart")
                    .WithPart("TitlePart")
                    .WithPart("AutoroutePart", builder => builder
                        .WithSetting("AutorouteSettings.AllowCustomPattern", "True")
                        .WithSetting("AutorouteSettings.AutomaticAdjustmentOnEdit", "False")
                        .WithSetting("AutorouteSettings.PatternDefinitions", "[{\"Name\":\"Title\",\"Pattern\":\"{Content.Slug}\",\"Description\":\"my-case\"}]"))
                    .WithPart("MenuPart")
                    .WithPart("AdminMenuPart", p => p.WithSetting("AdminMenuPartTypeSettings.DefaultPosition", "2"))
                );

            ContentDefinitionManager.AlterPartDefinition(typeof(CasePostPart).Name, builder => builder
                  .WithDescription("Turns content types into a CasePost."));

            ContentDefinitionManager.AlterTypeDefinition("CasePost",
                cfg => cfg
                    
                    .WithPart("CasePostPart")
                    .WithPart("CommonPart", p => p
                        .WithSetting("DateEditorSettings.ShowDateEditor", "True"))
                    .WithPart("PublishLaterPart")
                    .WithPart("TitlePart")
                    .WithPart("AutoroutePart", builder => builder
                        .WithSetting("AutorouteSettings.AllowCustomPattern", "True")
                        .WithSetting("AutorouteSettings.AutomaticAdjustmentOnEdit", "False")
                        .WithSetting("AutorouteSettings.PatternDefinitions", "[{\"Name\":\"Case and Title\",\"Pattern\":\"{Content.Container.Path}/{Content.Slug}\",\"Description\":\"my-case/my-post\"}]"))
                    .WithPart("BodyPart")
                    .WithPart(typeof(CasePostAttribPart).Name)
                    .Draftable()
                );

            ContentDefinitionManager.AlterPartDefinition("RecentCasePostsPart", part => part
                .WithDescription("Renders a list of recent case posts."));

            ContentDefinitionManager.AlterTypeDefinition("RecentCasePosts",
                cfg => cfg
                    .WithPart("RecentCasePostsPart")
                    .AsWidgetWithIdentity()
                );

            ContentDefinitionManager.AlterPartDefinition("CaseArchivesPart", part => part
                .WithDescription("Renders an archive of posts for a case."));

            ContentDefinitionManager.AlterTypeDefinition("CaseArchives",
                cfg => cfg
                    .WithPart("CaseArchivesPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                    .WithIdentity()
                );

            ContentDefinitionManager.AlterPartDefinition(typeof(CasePostAttribPart).Name, builder => builder
              .WithDescription("Case Attribut , Case number , Case year , case held @ and Parties "));
                     
                       
        SchemaBuilder.CreateTable(typeof(CasePostAttribRecord).Name,
                          table => table
                           .ContentPartVersionRecord()
                           .Column<Int64>("CasePostNumber", column => column.WithDefault(-1))     //, DbType.Int64)
                           .Column<Int16>("CaseYear", column=>column.NotNull())
                           .Column<string>("CaseDecision", column => column.WithLength(100))
                           .Column<string>("CaseClient1", column => column.WithLength(100))
                           .Column<string>("CaseClient2", column => column.WithLength(100))
                           .Column<string>("CaseHeldIn", column => column.WithLength(50))
                           )
                             .AlterTable(typeof(CasePostAttribRecord).Name,
                             table => table.
                             CreateIndex(typeof(CasePostAttribRecord).Name + "Idx", new string[] { "CasePostNumber", "CaseYear" })

                              );

            SchemaBuilder.CreateTable(typeof(CasePostHeldAtPartRecord).Name,
                         table => table
                        .ContentPartRecord()
                        .Column<string>("CourtName", column => column.NotNull())     //, DbType.Int64)
                          .Column<string>("CourtType", column => column.NotNull())//,DbType.Int16)
                       );

            ContentDefinitionManager.AlterPartDefinition(typeof(CasePostHeldAtPart).Name, builder => builder
              .WithDescription("Case Attribut , Case number , Case year , case held @ and Parties "));

            ContentDefinitionManager.AlterTypeDefinition("CasePostHeldAt",
                cfg => cfg
                      .WithPart(typeof(CasePostAttribPart).Name)
                      
               );
            SchemaBuilder.AlterTable(typeof(CasePostAttribRecord).Name,
                       table => table
                       .AddColumn<Int64>("CaseNumber")
                    );
            SchemaBuilder.CreateTable(typeof(CasePostStateRecord).Name,
                    table => table
                    .ContentPartRecord()
                    .Column<int>("CasePostPart_id")
                    .Column<string>("CasePostState")
                    .Column<Int64>("CasePostStateUserId")
                    .Column<DateTime>("CasePostStateUtc")
                    .Column<int>("CaseAcquiredStatus")
                 );

            SchemaBuilder.AlterTable(typeof(CasePostAttribRecord).Name,
                table => table
                        .AddColumn<string>("CaseAcquiredRole")

                     );
            SchemaBuilder.AlterTable(typeof(CasePostAttribRecord).Name,
                           table => table
                               .AddColumn<int>("CaseAcquiredBy")
                          );

            SchemaBuilder.AlterTable(typeof(CasePostAttribRecord).Name,
                                 table => table
                                 .AddColumn<string>("CaseDecisionTakenBy"));
            SchemaBuilder.AlterTable(typeof(CasePostAttribRecord).Name,
                                 table => table
                                 .AddColumn<string>("CaseHeldCourt"));
            SchemaBuilder.AlterTable(typeof(CasePostAttribRecord).Name,
                                table => table
                                .AddColumn<string>("CaseReference"));
            return 19;
        }

        public int UpdateFrom1() {
            ContentDefinitionManager.AlterTypeDefinition("Case", cfg => cfg.WithPart("AdminMenuPart", p => p.WithSetting("AdminMenuPartTypeSettings.DefaultPosition", "2")));
            return 2;
        }

        public int UpdateFrom2() {
            ContentDefinitionManager.AlterTypeDefinition("Case", cfg => cfg.WithPart("AdminMenuPart", p => p.WithSetting("AdminMenuPartTypeSettings.DefaultPosition", "2")));
            return 3;
        }

        public int UpdateFrom3() {
            ContentDefinitionManager.AlterTypeDefinition("Case", cfg => cfg.WithPart("CommonPart", p => p.WithSetting("DateEditorSettings.ShowDateEditor", "true")));
            return 4;
        }

        public int UpdateFrom4() {
            // adding the new fields required as Routable was removed
            // the user still needs to execute the corresponding migration
            // steps from the migration module

            SchemaBuilder.AlterTable("RecentCasePostsPartRecord", table => table
                   .AddColumn<int>("CaseNumber"));

       
            return 5;
        }

        public int UpdateFrom5() {
            ContentDefinitionManager.AlterPartDefinition("CasePart", builder => builder
                .WithDescription("Turns a content type into a Case."));

            ContentDefinitionManager.AlterPartDefinition("CasePostPart", builder => builder
                .WithDescription("Turns a content type into a CasePost."));

            ContentDefinitionManager.AlterPartDefinition("RecentCasePostsPart", part => part
                .WithDescription("Renders a list of recent case posts."));

            ContentDefinitionManager.AlterPartDefinition("CaseArchivesPart", part => part
                .WithDescription("Renders an archive of posts for a case."));

            return 6;
        }

        public int UpdateFrom6() {
            ContentDefinitionManager.AlterTypeDefinition("RecentCasePosts",
                cfg => cfg
                    .WithIdentity()
                );

            ContentDefinitionManager.AlterTypeDefinition("CaseArchives",
                cfg => cfg
                    .WithIdentity()
                );

           return 7;
       }

        public int UpdateFrom7()
        {
            ContentDefinitionManager.AlterPartDefinition(typeof(CasePostAttribPart).Name, builder => builder
               .WithDescription("Case Attribut , Case number , Case year , case held @ and Parties "));

            ContentDefinitionManager.AlterTypeDefinition("CasePost",
                cfg => cfg
                        .WithPart(typeof(CasePostAttribPart).Name)
                    
                );
            return 8;
        }
        public int UpdateFrom8() {
   
            SchemaBuilder.CreateTable(typeof(CasePostAttribRecord).Name,
                 table => table
                .ContentPartVersionRecord()
                .Column<Int64>("CasePostNumber", column => column.WithDefault(-1))     //, DbType.Int64)
                  .Column<Int16>("CaseYear")//,DbType.Int16)
                  
                    )
                    .AlterTable(typeof(CasePostAttribRecord).Name,
                    table => table.
                    CreateIndex(typeof(CasePostAttribRecord).Name + "Idx", new string[] { "CasePostNumber", "CaseYear" })

                     );

            return 9;

        }
        public int UpdateFrom9()
        {

            UpdateFrom8();

            return 10;

        }
        public int UpdateFrom10()
        {

            UpdateFrom8();

            return 11;

        }
        public int UpdateFrom11()
        {

            UpdateFrom8();

            return 12;

        }
        public int UpdateFrom12()
        {
            SchemaBuilder.CreateTable(typeof(CasePostAttribRecord).Name,
                           table => table
                          .ContentPartVersionRecord()
                          .Column<Int64>("CasePostNumber", column => column.WithDefault(-1))     //, DbType.Int64)
                            .Column<Int16>("CaseYear")//,DbType.Int16)

                              )
                              .AlterTable(typeof(CasePostAttribRecord).Name,
                              table => table.
                              CreateIndex(typeof(CasePostAttribRecord).Name + "Idx", new string[] { "CasePostNumber", "CaseYear" })

                               );

        
            return 13;

        }
        public int UpdateFrom13()
        {
            SchemaBuilder.CreateTable(typeof(CasePostHeldAtPartRecord).Name,
                           table => table
                          .ContentPartRecord()
                          .Column<string>("CourtName", column => column.NotNull())     //, DbType.Int64)
                            .Column<string>("CourtType", column => column.NotNull())//,DbType.Int16)
                         );

            ContentDefinitionManager.AlterPartDefinition(typeof(CasePostHeldAtPart).Name, builder => builder
              .WithDescription("Case Attribut , Case number , Case year , case held @ and Parties "));

            ContentDefinitionManager.AlterTypeDefinition("CasePostHeldAt",
                cfg => cfg
                        .WithPart(typeof(CasePostAttribPart).Name)

                );
            return 14;

        }
        public int UpdateFrom14()
        {
            SchemaBuilder.AlterTable(typeof(CasePostAttribRecord).Name,
                        table => table
                        .AddColumn<Int64>("CaseNumber")
                     );
            return 15;


        }
        public int UpdateFrom15()
        {
       
        SchemaBuilder.CreateTable(typeof(CasePostStateRecord).Name,
                        table => table
                        .ContentPartRecord()
                        .Column<int>("CasePostPart_id")
                        .Column<string>("CasePostState")
                        .Column<Int64>("CasePostStateUserId")
                        .Column<DateTime>("CasePostStateUtc")
                     );

            SchemaBuilder.AlterTable(typeof(CasePostAttribRecord).Name,
                table => table
                        .AddColumn<string>("CaseAcquiredRole")
                        
                     );
            SchemaBuilder.AlterTable(typeof(CasePostAttribRecord).Name,
                           table => table
                               .AddColumn<int>("CaseAcquiredBy")
                          );

            return 16;


        }

        public int Update16()
        {
            SchemaBuilder.AlterTable(typeof(CasePostStateRecord).Name,
                table => table
               .AddColumn<int>("CaseAcquiredStatus")
               );
            return 17;
        }
        public int Update17()
        {
            SchemaBuilder.AlterTable(typeof(CasePostStateRecord).Name,
                table => table
               .AddColumn<int>("CaseAcquiredStatus")
               );
            return 18;
        }
        public int Update18()
        {
            SchemaBuilder.AlterTable(typeof(CasePostAttribRecord).Name,
                                 table => table
                                 .AddColumn<string>("CaseDecisionTakenBy"));
            SchemaBuilder.AlterTable(typeof(CasePostAttribRecord).Name,
                                 table => table
                                 .AddColumn<string>("CaseHeldCourt"));
            SchemaBuilder.AlterTable(typeof(CasePostAttribRecord).Name,
                                table => table
                                .AddColumn<string>("CaseReference"));
            return 19;
        }



    }
}