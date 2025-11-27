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

                    if (hardNote.Composites?.Any() == true)
                    {
                        await InsertCompositesRecursive(connection, transaction, hardNote.Composites, hardNote.Id, parentCompositeId: null);
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

                await DeleteExistingComposites(connection, transaction, hardNote.Id);

                if (hardNote.Composites?.Any() == true)
                {
                    await InsertCompositesRecursive(connection, transaction, hardNote.Composites, hardNote.Id, parentCompositeId: null );
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
                var hardNotes = connection.Query<HardNote>(queryGetHardNotes).ToList();

                foreach (var hardNote in hardNotes)
                {
                    hardNote.Composites = LoadCompositesForHardNote(connection, hardNote.Id).ToList();
                }

                return hardNotes;
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
                hardNote.Composites = LoadCompositesForHardNote(connection, hardNote.Id).ToList();

                return hardNote;
            }
        }

        IEnumerable<CompositeBase> LoadCompositesForHardNote(IDbConnection connection, string hardNoteId)
        {
            var allComposites = LoadAllComposites(connection, hardNoteId);

            return BuildTree(allComposites);
        }
        List<CompositeBase> LoadAllComposites(IDbConnection connection, string hardNoteId)
        {
            var queryBase = "Select Id, HardNoteId, ParentId, CompositeType, Tag, Comment, OrderIndex From CompositeBase Where HardNoteId = @HardNoteId Order By OrderIndex";
            var baseComposites = connection.Query<CompositeBase>(queryBase, new { HardNoteId = hardNoteId }).ToList();

            if (!baseComposites.Any()) return new List<CompositeBase>();

            var compositeIds = baseComposites.Select(c => c.Id).ToList();

            var textComposites = LoadTextComposites(connection, compositeIds);
            var taskComposites = LoadTaskComposites(connection, compositeIds);
            var taskListComposites = LoadTaskListComposites(connection, compositeIds);
            var subTaskComposites = LoadSubTaskComposites(connection, compositeIds);
            var refComposites = LoadRefComposites(connection, compositeIds);
            var refListComposites = LoadRefListComposites(connection, compositeIds);
            var quoteComposites = LoadQuoteComposites(connection, compositeIds);
            var numericComposites = LoadNumericComposites(connection, compositeIds);
            var markerComposites = LoadMarkerComposites(connection, compositeIds);
            var lineComposites = LoadLineComposites(connection, compositeIds);
            var imageComposites = LoadImageComposites(connection, compositeIds);
            var headerComposites = LoadHeaderComposites(connection, compositeIds);
            var formattedTextComposites = LoadFormattedTextComposites(connection, compositeIds);
            var docComposites = LoadDocComposites(connection, compositeIds);
            var codeComposites = LoadCodeComposites(connection, compositeIds);

            var allComposites = new List<CompositeBase>();
            allComposites.AddRange(textComposites);
            allComposites.AddRange(taskComposites);
            allComposites.AddRange(taskListComposites);
            allComposites.AddRange(subTaskComposites);
            allComposites.AddRange(refComposites);
            allComposites.AddRange(refListComposites);
            allComposites.AddRange(quoteComposites);
            allComposites.AddRange(numericComposites);
            allComposites.AddRange(markerComposites);
            allComposites.AddRange(lineComposites);
            allComposites.AddRange(imageComposites);
            allComposites.AddRange(headerComposites);
            allComposites.AddRange(formattedTextComposites);
            allComposites.AddRange(docComposites);
            allComposites.AddRange(codeComposites);

            foreach (var composite in allComposites)
            {
                var baseInfo = baseComposites.FirstOrDefault(b => b.Id == composite.Id);
                if (baseInfo != null)
                {
                    composite.Tag = baseInfo.Tag;
                    composite.Comment = baseInfo.Comment;
                    composite.OrderIndex = baseInfo.OrderIndex;
                    composite.ParentId = baseInfo.ParentId;
                    composite.HardNoteId = baseInfo.HardNoteId;
                }
            }

            return allComposites;
        }
        List<CompositeBase> BuildTree(List<CompositeBase> allComposites)
        {
            if (!allComposites.Any()) return new List<CompositeBase>();

            var lookup = allComposites.ToLookup(c => string.IsNullOrEmpty(c.ParentId) ? null : c.ParentId);

            foreach (var composite in allComposites)
            {
                var childKey = string.IsNullOrEmpty(composite.Id) ? null : composite.Id;
                composite.Children = lookup[childKey].OrderBy(c => c.OrderIndex).ToList();
            }

            return lookup[null].OrderBy(c => c.OrderIndex).ToList();
        }

        List<TextComposite> LoadTextComposites(IDbConnection connection, List<string> ids)
        {
            if (!ids.Any()) return new List<TextComposite>();

            var query = "Select * From TextComposites Where Id IN @Ids";
            return connection.Query<TextComposite>(query, new { Ids = ids }).ToList();
        }
        List<TaskComposite> LoadTaskComposites(IDbConnection connection, List<string> ids)
        {
            if (!ids.Any()) return new List<TaskComposite>();

            var query = "Select * From TaskComposites Where Id IN @Ids";
            return connection.Query<TaskComposite>(query, new { Ids = ids }).ToList();
        }
        List<SubTaskComposite> LoadSubTaskComposites(IDbConnection connection, List<string> ids)
        {
            if (!ids.Any()) return new List<SubTaskComposite>();
        
            var query = "Select * From SubTaskComposites Where Id IN @Ids";
            return connection.Query<SubTaskComposite>(query, new { Ids = ids }).ToList();
        }
        List<RefComposite> LoadRefComposites(IDbConnection connection, List<string> ids)
        {
            if (!ids.Any()) return new List<RefComposite>();

            var query = "Select * From ReferenceComposites Where Id IN @Ids";
            return connection.Query<RefComposite>(query, new { Ids = ids }).ToList();
        }
        List<QuoteComposite> LoadQuoteComposites(IDbConnection connection, List<string> ids)
        {
            if (!ids.Any()) return new List<QuoteComposite>();

            var query = "Select * From QuoteComposites Where Id IN @Ids";
            return connection.Query<QuoteComposite>(query, new { Ids = ids }).ToList();
        }
        List<NumericComposite> LoadNumericComposites(IDbConnection connection, List<string> ids)
        {
            if (!ids.Any()) return new List<NumericComposite>();

            var query = "Select * From NumericComposites Where Id IN @Ids";
            return connection.Query<NumericComposite>(query, new { Ids = ids }).ToList();
        }
        List<MarkerComposite> LoadMarkerComposites(IDbConnection connection, List<string> ids)
        {
            if (!ids.Any()) return new List<MarkerComposite>();

            var query = "Select * From MarkerComposites Where Id IN @Ids";
            return connection.Query<MarkerComposite>(query, new { Ids = ids }).ToList();
        }
        List<LineComposite> LoadLineComposites(IDbConnection connection, List<string> ids)
        {
            if (!ids.Any()) return new List<LineComposite>();

            var query = "Select * From LineComposites Where Id IN @Ids";
            return connection.Query<LineComposite>(query, new { Ids = ids }).ToList();
        }
        List<ImageComposite> LoadImageComposites(IDbConnection connection, List<string> ids)
        {
            if (!ids.Any()) return new List<ImageComposite>();

            var query = "Select * From ImageComposites Where Id IN @Ids";
            return connection.Query<ImageComposite>(query, new { Ids = ids }).ToList();
        }
        List<HeaderComposite> LoadHeaderComposites(IDbConnection connection, List<string> ids)
        {
            if (!ids.Any()) return new List<HeaderComposite>();

            var query = "Select * From HeaderComposites Where Id IN @Ids";
            return connection.Query<HeaderComposite>(query, new { Ids = ids }).ToList();
        }
        List<FormattedTextComposite> LoadFormattedTextComposites(IDbConnection connection, List<string> ids)
        {
            if (!ids.Any()) return new List<FormattedTextComposite>();

            var query = "Select * From FormattedTextComposites Where Id IN @Ids";
            return connection.Query<FormattedTextComposite>(query, new { Ids = ids }).ToList();
        }
        List<DocComposite> LoadDocComposites(IDbConnection connection, List<string> ids)
        {
            if (!ids.Any()) return new List<DocComposite>();

            var query = "Select * From DocumentComposites Where Id IN @Ids";
            return connection.Query<DocComposite>(query, new { Ids = ids }).ToList();
        }
        List<CodeComposite> LoadCodeComposites(IDbConnection connection, List<string> ids)
        {
            if (!ids.Any()) return new List<CodeComposite>();

            var query = "Select * From CodeComposites Where Id IN @Ids";
            return connection.Query<CodeComposite>(query, new { Ids = ids }).ToList();
        }

        List<RefListComposite> LoadRefListComposites(IDbConnection connection, List<string> ids)
        {
            if (!ids.Any()) return new List<RefListComposite>();

            var query = "Select * From ReferencesComposites Where Id IN @Ids";
            return connection.Query<RefListComposite>(query, new { Ids = ids }).ToList();
        }
        List<TaskListComposite> LoadTaskListComposites(IDbConnection connection, List<string> ids)
        {
            if (!ids.Any()) return new List<TaskListComposite>();

            var query = "Select * From TasksComposites Where Id IN @Ids";
            return connection.Query<TaskListComposite>(query, new { Ids = ids }).ToList();
        }

        async Task DeleteExistingComposites(IDbConnection connection, IDbTransaction transaction, string hardNoteId)
        {
            var tptTables = new[]
            {
                "TextComposites",
                "TaskComposites",
                "SubTaskComposites",
                "ReferenceComposites",
                "QuoteComposites",
                "NumericComposites",
                "MarkerComposites",
                "LineComposites",
                "ImageComposites",
                "HeaderComposites",
                "FormattedTextComposites",
                "DocumentComposites",
                "CodeComposites",

                "ReferencesComposites",
                "TasksComposites"
            };

            foreach (var table in tptTables)
            {
                await connection.ExecuteAsync($"Delete From {table} Where Id In (Select Id From CompositeBase Where HardNoteId = @HardNoteId)", new { HardNoteId = hardNoteId }, transaction );
            }

            await connection.ExecuteAsync("Delete From CompositeBase Where HardNoteId = @HardNoteId", new { HardNoteId = hardNoteId }, transaction );
        }
        async Task InsertCompositesRecursive(IDbConnection connection, IDbTransaction transaction, IEnumerable<CompositeBase> composites, string hardNoteId, string parentCompositeId)
        {
            var tptMap = new Dictionary<Type, (string sql, Func<object, object> map)>
            {
                [typeof(TextComposite)] = ("Insert Into TextComposites(Id, Text) Values (@Id, @Text)", x => new
                {
                    ((TextComposite)x).Id,
                    ((TextComposite)x).Text
                }),
                [typeof(TaskComposite)] = ("Insert Into TaskComposites(Id, Completed, Text) Values (@Id, @Completed, @Text)", x => new
                {
                    ((TaskComposite)x).Id,
                    ((TaskComposite)x).Completed,
                    ((TaskComposite)x).Text
                }),
                [typeof(SubTaskComposite)] = ("Insert Into SubTaskComposites(Id, Text, Completed) Values (@Id, @Text, @Completed)", x => new
                {
                    ((SubTaskComposite)x).Id,
                    ((SubTaskComposite)x).Text,
                    ((SubTaskComposite)x).Completed
                }),
                [typeof(RefComposite)] = ("Insert Into ReferenceComposites(Id, Text, ValueRef) Values (@Id, @Text, @ValueRef)", x => new
                {
                    ((RefComposite)x).Id,
                    ((RefComposite)x).Text,
                    ((RefComposite)x).ValueRef
                }),
                [typeof(QuoteComposite)] = ("Insert Into QuoteComposites(Id, Text) Values (@Id, @Text)", x => new
                {
                    ((QuoteComposite)x).Id,
                    ((QuoteComposite)x).Text
                }),
                [typeof(NumericComposite)] = ("Insert Into NumericComposites(Id, Number, Text) Values (@Id, @Number, @Text)", x => new
                {
                    ((NumericComposite)x).Id,
                    ((NumericComposite)x).Number,
                    ((NumericComposite)x).Text
                }),
                [typeof(MarkerComposite)] = ("Insert Into MarkerComposites(Id, Text) Values (@Id, @Text)", x => new
                {
                    ((MarkerComposite)x).Id,
                    ((MarkerComposite)x).Text
                }),
                [typeof(LineComposite)] = ("Insert Into LineComposites(Id, LineSize, LineColor) Values (@Id, @LineSize, @LineColor)", x => new
                {
                    ((LineComposite)x).Id,
                    ((LineComposite)x).LineSize,
                    ((LineComposite)x).LineColor
                }),
                [typeof(ImageComposite)] = ("Insert Into ImageComposites(Id, HorizontalAlignment, Data) Values (@Id, @HorizontalAlignment, @Data)", x => new
                {
                    ((ImageComposite)x).Id,
                    ((ImageComposite)x).HorizontalAlignment,
                    ((ImageComposite)x).Data
                }),
                [typeof(HeaderComposite)] = ("Insert Into HeaderComposites(Id, Text, FontWeight, FontSize) Values (@Id, @Text, @FontWeight, @FontSize)", x => new
                {
                    ((HeaderComposite)x).Id,
                    ((HeaderComposite)x).Text,
                    ((HeaderComposite)x).FontWeight,
                    ((HeaderComposite)x).FontSize
                }),
                [typeof(FormattedTextComposite)] = ("Insert Into FormattedTextComposites(Id, BorderSize, CornerRadius, BorderColor, BackgroundColor, Data) Values (@Id, @BorderSize, @CornerRadius, @BorderColor, @BackgroundColor, @Data)", x => new
                {
                    ((FormattedTextComposite)x).Id,
                    ((FormattedTextComposite)x).BorderSize,
                    ((FormattedTextComposite)x).CornerRadius,
                    ((FormattedTextComposite)x).BorderColor,
                    ((FormattedTextComposite)x).BackgroundColor,
                    ((FormattedTextComposite)x).Data
                }),
                [typeof(DocComposite)] = ("Insert Into DocumentComposites(Id, Text, Data) Values (@Id, @Text, @Data)", x => new
                {
                    ((DocComposite)x).Id,
                    ((DocComposite)x).Text,
                    ((DocComposite)x).Data
                }),
                [typeof(CodeComposite)] = ("Insert Into CodeComposites(Id, Text) Values (@Id, @Text)", x => new
                {
                    ((CodeComposite)x).Id,
                    ((CodeComposite)x).Text
                }),

                [typeof(RefListComposite)] = ("Insert Into ReferencesComposites(Id) Values (@Id)", x => new
                {
                    ((RefListComposite)x).Id
                }),
                [typeof(TaskListComposite)] = ("Insert Into TasksComposites(Id, Text, Status, Completed) Values (@Id, @Text, @Status, @Completed)", x => new
                {
                    ((TaskListComposite)x).Id, ((TaskListComposite)x).Text, ((TaskListComposite)x).Status, ((TaskListComposite)x).Completed
                })
            };

            const string baseSql = "Insert Into CompositeBase(Id, HardNoteId, ParentId, CompositeType, Tag, Comment, OrderIndex) Values(@Id, @HardNoteId, @ParentId, @CompositeType, @Tag, @Comment, @OrderIndex)";

            foreach (var (composite, index) in composites.Select((c, i) => (c, i)))
            {
                composite.OrderIndex = index;
                composite.HardNoteId = hardNoteId;
                composite.ParentId = parentCompositeId;

                await connection.ExecuteAsync(baseSql, composite, transaction);

                if (tptMap.TryGetValue(composite.GetType(), out var handler))
                {
                    var (sql, map) = handler;
                    var parameters = map(composite);
                    await connection.ExecuteAsync(sql, parameters, transaction);
                }

                if (composite.Children?.Any() == true)
                {
                    await InsertCompositesRecursive(connection, transaction, composite.Children, hardNoteId, composite.Id);
                }
            }
        }
    }
}