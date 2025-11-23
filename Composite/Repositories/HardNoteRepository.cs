using Composite.Common.Factories;
using Composite.Models;
using Composite.Models.Notes.HardNote;
using Dapper;
using System.Data;

namespace Composite.Repositories
{
    public class HardNoteRepository(IDbConnectionFactory dbConnectionFactory) : IHardNoteRepository
    {
        public async Task<bool> Create(HardNote hardNote)
        {
            using (var connection = dbConnectionFactory.CreateConnection())
            {
                connection.Open();

                using var transaction = connection.BeginTransaction();

                try
                {
                    var queryAddHardNote = "Insert Into HardNotes(Id, Title, Category, DateCreate, Password) Values (@Id, @Title, @Category, @DateCreate, @Password)";
                    var resultAddHardNote = await connection.ExecuteAsync(queryAddHardNote, hardNote, transaction);

                    if(hardNote.Composites?.Any() == true)
                    {
                        var tptMap = new Dictionary<Type, (string sql, Func<object, object> Map)>
                        {
                            [typeof(TextComposite)] = (@"Insert Into TextComposites(Id, Text) Values (@Id, @Text)", x => new
                            {
                                ((TextComposite)x).Id,
                                ((TextComposite)x).Text
                            }),
                            [typeof(TaskComposite)] = (@"Insert Into TaskComposites(Id, Completed, Text) Values (@Id, @Completed, @Text)", x => new
                            {
                                ((TaskComposite)x).Id,
                                ((TaskComposite)x).Completed,
                                ((TaskComposite)x).Text
                            }),
                            [typeof(RefComposite)] = (@"Insert Into ReferenceComposites(Id, Text, ValueRef) Values (@Id, @Text, @ValueRef)", x => new
                            {
                                ((RefComposite)x).Id,
                                ((RefComposite)x).Text,
                                ((RefComposite)x).ValueRef
                            }),
                            [typeof(QuoteComposite)] = (@"Insert Into QuoteComposites(Id, Text) Values (@Id, @Text)", x => new
                            {
                                ((QuoteComposite)x).Id,
                                ((QuoteComposite)x).Text
                            }),
                            [typeof(NumericComposite)] = (@"Insert Into NumericComposites(Id, Number, Text) Values (@Id, @Number, @Text)", x => new
                            {
                                ((NumericComposite)x).Id,
                                ((NumericComposite)x).Number,
                                ((NumericComposite)x).Text
                            }),
                            [typeof(MarkerComposite)] = (@"Insert Into MarkerComposites(Id, Text) Values (@Id, @Text)", x => new
                            {
                                ((MarkerComposite)x).Id,
                                ((MarkerComposite)x).Text
                            }),
                            [typeof(LineComposite)] = (@"Insert Into LineComposites(Id, LineSize, LineColor) Values (@Id, @LineSize, @LineColor)", x => new
                            {
                                ((LineComposite)x).Id,
                                ((LineComposite)x).LineSize,
                                ((LineComposite)x).LineColor
                            }),
                            [typeof(ImageComposite)] = (@"Insert Into ImageComposites(Id, HorizontalAlignment, Data) Values (@Id, @HorizontalAlignment, @Data)", x => new
                            {
                                ((ImageComposite)x).Id,
                                ((ImageComposite)x).HorizontalAlignment,
                                ((ImageComposite)x).Data
                            }),
                            [typeof(HeaderComposite)] = (@"Insert Into HeaderComposites(Id, Text, FontWeight, FontSize) Values (@Id, @Text, @FontWeight, @FontSize)", x => new
                            {
                                ((HeaderComposite)x).Id,
                                ((HeaderComposite)x).Text,
                                ((HeaderComposite)x).FontWeight,
                                ((HeaderComposite)x).FontSize
                            }),
                            [typeof(FormattedTextComposite)] = (@"Insert Into FormattedTextComposites(Id, BorderSize, CornerRadius, BorderColor, BackgroundColor, Data) Values (@Id, @BorderSize, @CornerRadius, @BorderColor, @BackgroundColor, @Data)", x => new
                            {
                                ((FormattedTextComposite)x).Id,
                                ((FormattedTextComposite)x).BorderSize,
                                ((FormattedTextComposite)x).CornerRadius,
                                ((FormattedTextComposite)x).BorderColor,
                                ((FormattedTextComposite)x).BackgroundColor,
                                ((FormattedTextComposite)x).Data
                            }),
                            [typeof(DocComposite)] = (@"Insert Into DocumentComposites(Id, Text, Data) Values (@Id, @Text, @Data)", x => new
                            {
                                ((DocComposite)x).Id,
                                ((DocComposite)x).Text,
                                ((DocComposite)x).Data
                            }),
                            [typeof(CodeComposite)] = (@"Insert Into CodeComposites(Id, Text) Values (@Id, @Text)", x => new
                            {
                                ((CodeComposite)x).Id,
                                ((CodeComposite)x).Text
                            })
                        };

                        const string baseSql = "Insert Into CompositeBase(Id, HardNoteId, CompositeType, Tag, Comment, OrderIndex) Values(@Id, @HardNoteId, @CompositeType, @Tag, @Comment, @OrderIndex)";

                        foreach (var (composite, index) in hardNote.Composites.Select((c, i) => (c, i)))
                        {
                            composite.OrderIndex = index;

                            await connection.ExecuteAsync(baseSql, composite, transaction);

                            if (tptMap.TryGetValue(composite.GetType(), out var handler))
                            {
                                var (sql, map) = handler;
                                var parameters = map(composite);

                                await connection.ExecuteAsync(sql, parameters, transaction);
                            }
                        }
                    }

                    transaction.Commit();
                    return resultAddHardNote > 0;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        public async Task<bool> Update(HardNote hardNote)
        {
            using var connection = dbConnectionFactory.CreateConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                var queryUpdateHardNote = "Update HardNotes Set Title = @Title, DateCreate = @DateCreate, Category = @Category, Password = @Password Where Id = @Id";
                var resultHardNote = await connection.ExecuteAsync(queryUpdateHardNote, hardNote, transaction);

                if (resultHardNote == 0)
                {
                    transaction.Rollback();
                    return false;
                }

                await connection.ExecuteAsync("Delete From CompositeBase Where HardNoteId = @Id", new { Id = hardNote.Id }, transaction);

                var tptTables = new[]
                {
                    "TextComposites",
                    "TaskComposites",
                    "ReferenceComposites",
                    "QuoteComposites",
                    "NumericComposites",
                    "MarkerComposites",
                    "LineComposites",
                    "ImageComposites",
                    "HeaderComposites",
                    "FormattedTextComposites",
                    "DocumentComposites",
                    "CodeComposites"
                };

                foreach (var table in tptTables)
                {
                    await connection.ExecuteAsync($"Delete From {table} Where Id In (Select Id From CompositeBase Where HardNoteId = @HardNoteId)", new { HardNoteId = hardNote.Id }, transaction);
                }

                if (hardNote.Composites == null || hardNote.Composites.Count == 0)
                {
                    transaction.Commit();
                    return true;
                }

                var tptMap = new Dictionary<Type, (string sql, Func<object, object> Map)>
                {
                    [typeof(TextComposite)] = (@"Insert Into TextComposites(Id, Text) Values (@Id, @Text)", x => new
                    {
                        ((TextComposite)x).Id,
                        ((TextComposite)x).Text
                    }),
                    [typeof(TaskComposite)] = (@"Insert Into TaskComposites(Id, Completed, Text) Values (@Id, @Completed, @Text)", x => new
                    {
                        ((TaskComposite)x).Id,
                        ((TaskComposite)x).Completed,
                        ((TaskComposite)x).Text
                    }),
                    [typeof(RefComposite)] = (@"Insert Into ReferenceComposites(Id, Text, ValueRef) Values (@Id, @Text, @ValueRef)", x => new
                    {
                        ((RefComposite)x).Id,
                        ((RefComposite)x).Text,
                        ((RefComposite)x).ValueRef
                    }),
                    [typeof(QuoteComposite)] = (@"Insert Into QuoteComposites(Id, Text) Values (@Id, @Text)", x => new
                    {
                        ((QuoteComposite)x).Id,
                        ((QuoteComposite)x).Text
                    }),
                    [typeof(NumericComposite)] = (@"Insert Into NumericComposites(Id, Number, Text) Values (@Id, @Number, @Text)", x => new
                    {
                        ((NumericComposite)x).Id,
                        ((NumericComposite)x).Number,
                        ((NumericComposite)x).Text
                    }),
                    [typeof(MarkerComposite)] = (@"Insert Into MarkerComposites(Id, Text) Values (@Id, @Text)", x => new
                    {
                        ((MarkerComposite)x).Id,
                        ((MarkerComposite)x).Text
                    }),
                    [typeof(LineComposite)] = (@"Insert Into LineComposites(Id, LineSize, LineColor) Values (@Id, @LineSize, @LineColor)", x => new
                    {
                        ((LineComposite)x).Id,
                        ((LineComposite)x).LineSize,
                        ((LineComposite)x).LineColor
                    }),
                    [typeof(ImageComposite)] = (@"Insert Into ImageComposites(Id, HorizontalAlignment, Data) Values (@Id, @HorizontalAlignment, @Data)", x => new
                    {
                        ((ImageComposite)x).Id,
                        ((ImageComposite)x).HorizontalAlignment,
                        ((ImageComposite)x).Data
                    }),
                    [typeof(HeaderComposite)] = (@"Insert Into HeaderComposites(Id, Text, FontWeight, FontSize) Values (@Id, @Text, @FontWeight, @FontSize)", x => new
                    {
                        ((HeaderComposite)x).Id,
                        ((HeaderComposite)x).Text,
                        ((HeaderComposite)x).FontWeight,
                        ((HeaderComposite)x).FontSize
                    }),
                    [typeof(FormattedTextComposite)] = (@"Insert Into FormattedTextComposites(Id, BorderSize, CornerRadius, BorderColor, BackgroundColor, Data) Values (@Id, @BorderSize, @CornerRadius, @BorderColor, @BackgroundColor, @Data)", x => new
                    {
                        ((FormattedTextComposite)x).Id,
                        ((FormattedTextComposite)x).BorderSize,
                        ((FormattedTextComposite)x).CornerRadius,
                        ((FormattedTextComposite)x).BorderColor,
                        ((FormattedTextComposite)x).BackgroundColor,
                        ((FormattedTextComposite)x).Data
                    }),
                    [typeof(DocComposite)] = (@"Insert Into DocumentComposites(Id, Text, Data) Values (@Id, @Text, @Data)", x => new
                    {
                        ((DocComposite)x).Id,
                        ((DocComposite)x).Text,
                        ((DocComposite)x).Data
                    }),
                    [typeof(CodeComposite)] = (@"Insert Into CodeComposites(Id, Text) Values (@Id, @Text)", x => new
                    {
                        ((CodeComposite)x).Id,
                        ((CodeComposite)x).Text
                    })
                };

                const string baseSql = "Insert Into CompositeBase(Id, HardNoteId, CompositeType, Tag, Comment, OrderIndex) Values (@Id, @HardNoteId, @CompositeType, @Tag, @Comment, @OrderIndex)";
                   
                foreach (var (composite, index) in hardNote.Composites.Select((c, i) => (c, i)))
                {
                    composite.OrderIndex = index;

                    await connection.ExecuteAsync(baseSql, composite, transaction);

                    if (tptMap.TryGetValue(composite.GetType(), out var handler))
                    {
                        var (sql, map) = handler;
                        var parameters = map(composite);

                        await connection.ExecuteAsync(sql, parameters, transaction);
                    }
                }

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        public async Task<bool> Delete(string id)
        {
            using (var connection = dbConnectionFactory.CreateConnection())
            {
                connection.Open();

                var queryDeleteHardNote = "Delete From HardNotes Where Id = @id";
                var resultDeleteHardNote = await connection.ExecuteAsync(queryDeleteHardNote, new { id });

                var queryDeleteRefCompositeIdNote = "Delete From ReferenceComposites Where ValueRef = @id";
                var resultDeleteRefCompositeIdNote = await connection.ExecuteAsync(queryDeleteRefCompositeIdNote, new { id });

                if (resultDeleteHardNote > 0) return true;

                return false;
            }
        }
        public IEnumerable<HardNote> Read()
        {
            using (var connection = dbConnectionFactory.CreateConnection())
            {
                connection.Open();

                var queryGetHardNotes = "Select * From HardNotes";
                var resultGetHardNotes = connection.Query<HardNote>(queryGetHardNotes).ToList();

                foreach (var hardNote in resultGetHardNotes) hardNote.Composites = GetComposites(connection, hardNote.Id);

                return resultGetHardNotes;
            }
        }
        public IEnumerable<HardNote> GetIdTitleNotes()
        {
            using (var connection = dbConnectionFactory.CreateConnection())
            {
                connection.Open();

                var queryGetIdTitleHardNotes = "Select Id, Title From HardNotes";
                var resultGetIdTitleHardNotes = connection.Query<HardNote>(queryGetIdTitleHardNotes);

                return resultGetIdTitleHardNotes;
            }
        }
        public async Task<HardNote> GetNoteById(string id)
        {
            using (var connection = dbConnectionFactory.CreateConnection())
            {
                connection.Open();

                var queryGetHardNoteById = "Select * From HardNotes Where Id = @Id";
                var hardNote = await connection.QueryFirstOrDefaultAsync<HardNote>(queryGetHardNoteById, new { Id = id });

                var queryGetComposites = "Select * From CompositeBase Where HardNoteId = @HardNoteId Order By OrderIndex";
                hardNote.Composites = GetComposites(connection, id);

                return hardNote;
            }
        }

        List<CompositeBase> GetComposites(IDbConnection connection, string hardNoteId)
        {
            var composites = new List<CompositeBase>();
            var queryGetCompositesBase = "Select * From CompositeBase Where HardNoteId = @HardNoteId Order By OrderIndex";
            var compositeBases = connection.Query<CompositeBase>(queryGetCompositesBase, new { HardNoteId = hardNoteId }).ToList();

            if (!compositeBases.Any()) return composites;

            var ids = compositeBases.Select(x => x.Id).ToList();

            var textComposites = GetTextComposites(connection, ids);
            var headerComposites = GetHeaderComposites(connection, ids);
            var quoteComposites = GetQuoteComposites(connection, ids);
            var codeComposites = GetCodeComposites(connection, ids);
            var markerComposites = GetMarkerComposites(connection, ids);
            var numericComposites = GetNumericComposites(connection, ids);
            var taskComposites = GetTaskComposites(connection, ids);
            var lineComposites = GetLineComposites(connection, ids);
            var documentComposites = GetDocumentComposites(connection, ids);
            var referenceComposites = GetReferenceComposites(connection, ids);
            var imageComposites = GetImageComposites(connection, ids);
            var formattedTextComposites = GetFormattedTextComposites(connection, ids);

            var textDict = textComposites.ToDictionary(c => c.Id);
            var headerDict = headerComposites.ToDictionary(c => c.Id);
            var quoteDict = quoteComposites.ToDictionary(c => c.Id);
            var codeDict = codeComposites.ToDictionary(c => c.Id);
            var markerDict = markerComposites.ToDictionary(c => c.Id);
            var numericDict = numericComposites.ToDictionary(c => c.Id);
            var taskDict = taskComposites.ToDictionary(c => c.Id);
            var lineDict = lineComposites.ToDictionary(c => c.Id);
            var documentDict = documentComposites.ToDictionary(c => c.Id);
            var referenceDict = referenceComposites.ToDictionary(c => c.Id);
            var imageDict = imageComposites.ToDictionary(c => c.Id);
            var formattedDict = formattedTextComposites.ToDictionary(c => c.Id);

            foreach (var baseComposite in compositeBases)
            {
                CompositeBase composite = baseComposite.CompositeType switch
                {
                    nameof(TextComposite) => MapComposite(baseComposite, textDict),
                    nameof(HeaderComposite) => MapComposite(baseComposite, headerDict),
                    nameof(QuoteComposite) => MapComposite(baseComposite, quoteDict),
                    nameof(CodeComposite) => MapComposite(baseComposite, codeDict),
                    nameof(MarkerComposite) => MapComposite(baseComposite, markerDict),
                    nameof(NumericComposite) => MapComposite(baseComposite, numericDict),
                    nameof(TaskComposite) => MapComposite(baseComposite, taskDict),
                    nameof(LineComposite) => MapComposite(baseComposite, lineDict),
                    nameof(DocComposite) => MapComposite(baseComposite, documentDict),
                    nameof(RefComposite) => MapComposite(baseComposite, referenceDict),
                    nameof(ImageComposite) => MapComposite(baseComposite, imageDict),
                    nameof(FormattedTextComposite) => MapComposite(baseComposite, formattedDict),
                    _ => baseComposite
                };

                composites.Add(composite);
            }

            return composites;
        }

        T MapComposite<T>(CompositeBase baseComposite, Dictionary<string, T> dict) where T : CompositeBase
        {
            if (dict.TryGetValue(baseComposite.Id, out var specific))
            {
                specific.HardNoteId = baseComposite.HardNoteId;
                specific.CompositeType = baseComposite.CompositeType;
                specific.Tag = baseComposite.Tag;
                specific.Comment = baseComposite.Comment;
                specific.OrderIndex = baseComposite.OrderIndex;
                return specific;
            }
            return null;
        }
        List<TextComposite> GetTextComposites(IDbConnection connection, List<string> ids)
        {
            var query = "Select * From TextComposites Where Id IN @Ids";
            return connection.Query<TextComposite>(query, new { Ids = ids }).ToList();
        }
        List<HeaderComposite> GetHeaderComposites(IDbConnection connection, List<string> ids)
        {
            var query = "Select * From HeaderComposites Where Id IN @Ids";
            return connection.Query<HeaderComposite>(query, new { Ids = ids }).ToList();
        }
        List<QuoteComposite> GetQuoteComposites(IDbConnection connection, List<string> ids)
        {
            var query = "Select * From QuoteComposites Where Id IN @Ids";
            return connection.Query<QuoteComposite>(query, new { Ids = ids }).ToList();
        }
        List<CodeComposite> GetCodeComposites(IDbConnection connection, List<string> ids)
        {
            var query = "Select * From CodeComposites Where Id IN @Ids";
            return connection.Query<CodeComposite>(query, new { Ids = ids }).ToList();
        }
        List<MarkerComposite> GetMarkerComposites(IDbConnection connection, List<string> ids)
        {
            var query = "Select * From MarkerComposites Where Id IN @Ids";
            return connection.Query<MarkerComposite>(query, new { Ids = ids }).ToList();
        }
        List<NumericComposite> GetNumericComposites(IDbConnection connection, List<string> ids)
        {
            var query = "Select * From NumericComposites Where Id IN @Ids";
            return connection.Query<NumericComposite>(query, new { Ids = ids }).ToList();
        }
        List<TaskComposite> GetTaskComposites(IDbConnection connection, List<string> ids)
        {
            var query = "Select * From TaskComposites Where Id IN @Ids";
            return connection.Query<TaskComposite>(query, new { Ids = ids }).ToList();
        }
        List<LineComposite> GetLineComposites(IDbConnection connection, List<string> ids)
        {
            var query = "Select * From LineComposites Where Id IN @Ids";
            return connection.Query<LineComposite>(query, new { Ids = ids }).ToList();
        }
        List<DocComposite> GetDocumentComposites(IDbConnection connection, List<string> ids)
        {
            var query = "Select * From DocumentComposites Where Id IN @Ids";
            return connection.Query<DocComposite>(query, new { Ids = ids }).ToList();
        }
        List<RefComposite> GetReferenceComposites(IDbConnection connection, List<string> ids)
        {
            var query = "Select * From ReferenceComposites Where Id IN @Ids";
            return connection.Query<RefComposite>(query, new { Ids = ids }).ToList();
        }
        List<ImageComposite> GetImageComposites(IDbConnection connection, List<string> ids)
        {
            var query = "Select * From ImageComposites Where Id IN @Ids";
            return connection.Query<ImageComposite>(query, new { Ids = ids }).ToList();
        }
        List<FormattedTextComposite> GetFormattedTextComposites(IDbConnection connection, List<string> ids)
        {
            var query = "Select * From FormattedTextComposites Where Id IN @Ids";
            return connection.Query<FormattedTextComposite>(query, new { Ids = ids }).ToList();
        }
    }
}