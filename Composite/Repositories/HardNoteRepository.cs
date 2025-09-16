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

                var queryAddHardNote = "Insert Into HardNotes(Id, Title, Category) Values (@Id, @Title, @Category)";
                var resultAddHardNote = await connection.ExecuteAsync(queryAddHardNote, hardNote);

                if (hardNote.Composites?.Any() == true)
                {
                    var textComposites = hardNote.Composites.OfType<TextComposite>().ToList();
                    var headerComposites = hardNote.Composites.OfType<HeaderComposite>().ToList();
                    var quoteComposites = hardNote.Composites.OfType<QuoteComposite>().ToList();
                    var lineComposites = hardNote.Composites.OfType<LineComposite>().ToList();

                    if (textComposites.Any())
                    {
                        var queryTexts = @"Insert Into Composites(Id, Tag, Comment, Text, HardNoteId, CompositeType) 
                                         Values (@Id, @Tag, @Comment, @Text, @HardNoteId, @CompositeType)";

                        await connection.ExecuteAsync(queryTexts, textComposites);
                    }
                    if (headerComposites.Any())
                    {
                        var queryHeaders = @"Insert Into Composites(Id, Tag, Comment, Header, FontWeightHeader, FontSizeHeader, HardNoteId, CompositeType) 
                                         Values (@Id, @Tag, @Comment, @Header, @FontWeightHeader, @FontSizeHeader, @HardNoteId, @CompositeType)";

                        await connection.ExecuteAsync(queryHeaders, headerComposites);
                    }
                    if (quoteComposites.Any())
                    {
                        var queryQuotes = @"Insert Into Composites(Id, Tag, Comment, Quote, HardNoteId, CompositeType) 
                                         Values (@Id, @Tag, @Comment, @Quote, @HardNoteId, @CompositeType)";

                        await connection.ExecuteAsync(queryQuotes, quoteComposites);
                    }
                    if (lineComposites.Any())
                    {
                        var queryLines = @"Insert Into Composites(Id, Tag, Comment, HardNoteId, CompositeType) 
                                         Values (@Id, @Tag, @Comment, @HardNoteId, @CompositeType)";

                        await connection.ExecuteAsync(queryLines, lineComposites);
                    }
                }

                if (resultAddHardNote > 0) return true;
            }

            return false;
        }
        public async Task<bool> Update(HardNote hardNote)
        {
            using (var connection = dbConnectionFactory.CreateConnection())
            {
                connection.Open();
                
                 var queryUpdateHardNote = "Update HardNotes Set Title = @Title, Category = @Category Where Id = @Id";
                 var resultUpdateHardNote = await connection.ExecuteAsync(queryUpdateHardNote, hardNote);

                 if (resultUpdateHardNote == 0) return false;

                 var queryDeleteComposites = "Delete From Composites Where HardNoteId = @Id";
                 await connection.ExecuteAsync(queryDeleteComposites, new { Id = hardNote.Id });

                  if (hardNote.Composites?.Count > 0)
                  {
                      var queryInsertComposites = @"Insert Into Composites (Id, Tag, Comment, Text, HardNoteId, CompositeType) 
                                                  Values (@Id, @Tag, @Comment, @Text, @HardNoteId, @CompositeType)";

                      var compositeData = hardNote.Composites.Select(c => new
                      {
                          Id = c.Id,
                          Tag = c.Tag,
                          Comment = c.Comment,
                          Text = c.Text,
                          HardNoteId = hardNote.Id,
                          CompositeType = c.CompositeType
                      });

                      var resultInsertComposites = await connection.ExecuteAsync(queryInsertComposites, compositeData);

                      if (resultInsertComposites != hardNote.Composites.Count) return false;
                  }

                  return true;
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
                    var queryGetComposites = "Select * From Composites Where HardNoteId = @HardNoteId";
                    var composites = connection.Query<CompositeBase>(queryGetComposites, new { HardNoteId = hardNote.Id });
                    hardNote.Composites = composites.ToList();
                }

                return resultGetHardNotes;
            }
        }
    }
}