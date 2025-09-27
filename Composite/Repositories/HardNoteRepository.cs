using Composite.Common.Factories;
using Composite.Models;
using Composite.Models.Notes.HardNote;
using Dapper;

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
                    var queryAddHardNote = "Insert Into HardNotes(Id, Title, Category, DateCreate) Values (@Id, @Title, @Category, @DateCreate)";
                    var resultAddHardNote = await connection.ExecuteAsync(queryAddHardNote, hardNote, transaction);

                    if (hardNote.Composites?.Any() == true)
                    {
                        // Создаем список с порядковыми номерами
                        var compositesWithOrder = hardNote.Composites
                            .Select((composite, index) => new { Composite = composite, OrderIndex = index })
                            .ToList();

                        // TextComposite
                        var textComposites = compositesWithOrder
                            .Where(x => x.Composite is TextComposite)
                            .Select(x => new
                            {
                                ((TextComposite)x.Composite).Id,
                                ((TextComposite)x.Composite).Tag,
                                ((TextComposite)x.Composite).Comment,
                                ((TextComposite)x.Composite).Text,
                                ((TextComposite)x.Composite).HardNoteId,
                                ((TextComposite)x.Composite).CompositeType,
                                x.OrderIndex
                            })
                            .ToList();
                        if (textComposites.Any())
                        {
                            var queryTexts = @"Insert Into Composites(Id, Tag, Comment, Text, HardNoteId, CompositeType, OrderIndex) 
                                 Values (@Id, @Tag, @Comment, @Text, @HardNoteId, @CompositeType, @OrderIndex)";
                            await connection.ExecuteAsync(queryTexts, textComposites, transaction);
                        }


                        var headerComposites = compositesWithOrder
                            .Where(x => x.Composite is HeaderComposite)
                            .Select(x => new
                            {
                                ((HeaderComposite)x.Composite).Id,
                                ((HeaderComposite)x.Composite).Tag,
                                ((HeaderComposite)x.Composite).Comment,
                                ((HeaderComposite)x.Composite).Header,
                                ((HeaderComposite)x.Composite).FontWeightHeader,
                                ((HeaderComposite)x.Composite).FontSizeHeader,
                                ((HeaderComposite)x.Composite).HardNoteId,
                                ((HeaderComposite)x.Composite).CompositeType,
                                x.OrderIndex
                            })
                            .ToList();
                        if (headerComposites.Any())
                        {
                            var queryHeaders = @"Insert Into Composites(Id, Tag, Comment, Header, FontWeightHeader, FontSizeHeader, HardNoteId, CompositeType, OrderIndex) 
                                   Values (@Id, @Tag, @Comment, @Header, @FontWeightHeader, @FontSizeHeader, @HardNoteId, @CompositeType, @OrderIndex)";
                            await connection.ExecuteAsync(queryHeaders, headerComposites, transaction);
                        }

                        var quoteComposites = compositesWithOrder
                           .Where(x => x.Composite is QuoteComposite)
                           .Select(x => new
                           {
                               ((QuoteComposite)x.Composite).Id,
                               ((QuoteComposite)x.Composite).Tag,
                               ((QuoteComposite)x.Composite).Comment,
                               ((QuoteComposite)x.Composite).Quote,
                               ((QuoteComposite)x.Composite).HardNoteId,
                               ((QuoteComposite)x.Composite).CompositeType,
                               x.OrderIndex
                           })
                           .ToList();
                        if (quoteComposites.Any())
                        {
                            var queryQuotes = @"Insert Into Composites(Id, Tag, Comment, Quote, HardNoteId, CompositeType, OrderIndex) 
                                   Values (@Id, @Tag, @Comment, @Quote, @HardNoteId, @CompositeType, @OrderIndex)";
                            await connection.ExecuteAsync(queryQuotes, quoteComposites, transaction);
                        }

                        var taskComposites = compositesWithOrder
                           .Where(x => x.Composite is TaskComposite)
                           .Select(x => new
                           {
                               ((TaskComposite)x.Composite).Id,
                               ((TaskComposite)x.Composite).Tag,
                               ((TaskComposite)x.Composite).Comment,
                               ((TaskComposite)x.Composite).TaskText,
                               ((TaskComposite)x.Composite).Completed,
                               ((TaskComposite)x.Composite).HardNoteId,
                               ((TaskComposite)x.Composite).CompositeType,
                               x.OrderIndex
                           })
                           .ToList();
                        if (taskComposites.Any())
                        {
                            var queryTasks = @"Insert Into Composites(Id, Tag, Comment, TaskText, Completed, HardNoteId, CompositeType, OrderIndex) 
                                   Values (@Id, @Tag, @Comment, @TaskText, @Completed, @HardNoteId, @CompositeType, @OrderIndex)";
                            await connection.ExecuteAsync(queryTasks, taskComposites, transaction);
                        }

                        var lineComposites = compositesWithOrder
                           .Where(x => x.Composite is LineComposite)
                           .Select(x => new
                           {
                               ((LineComposite)x.Composite).Id,
                               ((LineComposite)x.Composite).Tag,
                               ((LineComposite)x.Composite).Comment,
                               ((LineComposite)x.Composite).HardNoteId,
                               ((LineComposite)x.Composite).CompositeType,
                               x.OrderIndex
                           })
                           .ToList();
                        if (lineComposites.Any())
                        {
                            var queryLines = @"Insert Into Composites(Id, Tag, Comment, HardNoteId, CompositeType, OrderIndex) 
                                   Values (@Id, @Tag, @Comment, @HardNoteId, @CompositeType, @OrderIndex)";
                            await connection.ExecuteAsync(queryLines, lineComposites, transaction);
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
            using (var connection = dbConnectionFactory.CreateConnection())
            {
                connection.Open();
                using var transaction = connection.BeginTransaction();

                try
                {
                    var queryUpdateHardNote = "Update HardNotes Set Title = @Title, Category = @Category Where Id = @Id";
                    var resultUpdateHardNote = await connection.ExecuteAsync(queryUpdateHardNote, hardNote, transaction);

                    if (resultUpdateHardNote == 0)
                    {
                        transaction.Rollback();
                        return false;
                    }

                    var queryDeleteComposites = "Delete From Composites Where HardNoteId = @Id";
                    await connection.ExecuteAsync(queryDeleteComposites, new { Id = hardNote.Id }, transaction);

                    if (hardNote.Composites?.Count > 0)
                    {
                        var queryInsertComposites = @"Insert Into Composites (Id, Tag, Comment, Text, Header, FontWeightHeader, FontSizeHeader, Quote, TaskText, Completed, HardNoteId, CompositeType, OrderIndex)
                                                      Values (@Id, @Tag, @Comment, @Text, @Header, @FontWeightHeader, @FontSizeHeader, @Quote, @TaskText, @Completed, @HardNoteId, @CompositeType, @OrderIndex)";

                        var compositeData = hardNote.Composites.Select((c, index) => new
                        {
                            Id = c.Id,
                            Tag = c.Tag,
                            Comment = c.Comment,
                            Text = c.Text,
                            Header = c.Header,
                            FontWeightHeader = c.FontWeightHeader,
                            FontSizeHeader = c.FontSizeHeader,
                            Quote = c.Quote,
                            TaskText = c.TaskText,
                            Completed = c.Completed,
                            HardNoteId = hardNote.Id,
                            CompositeType = c.CompositeType,
                            OrderIndex = index  // Добавляем порядковый номер
                        });

                        var resultInsertComposites = await connection.ExecuteAsync(queryInsertComposites, compositeData, transaction);

                        if (resultInsertComposites != hardNote.Composites.Count)
                        {
                            transaction.Rollback();
                            return false;
                        }
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        public async Task<bool> Delete(string id)
        {
            using (var connection = dbConnectionFactory.CreateConnection())
            {
                connection.Open();

                var queryDeleteHardNote = "Delete From HardNotes Where Id = @id";
                var resultDeleteHardNote = await connection.ExecuteAsync(queryDeleteHardNote, new { id });

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

                foreach (var hardNote in resultGetHardNotes)
                {
                    var queryGetComposites = "Select * From Composites Where HardNoteId = @HardNoteId Order By OrderIndex";
                    var composites = connection.Query<CompositeBase>(queryGetComposites, new { HardNoteId = hardNote.Id });
                    hardNote.Composites = composites.ToList();
                }

                return resultGetHardNotes;
            }
        }
    }
}