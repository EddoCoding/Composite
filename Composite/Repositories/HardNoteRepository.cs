using System.Data.Common;
using System.Transactions;
using Composite.Common.Factories;
using Composite.Models;
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

                var queryAddHardNote = "INSERT INTO HardNotes(Id, Category) VALUES (@Id, @Category)";
                var resultAddHardNote = await connection.ExecuteAsync(queryAddHardNote, hardNote);

                if (hardNote.Composites?.Any() == true)
                {
                    var headerComposites = hardNote.Composites.OfType<HeaderComposite>().ToList();
                    var textComposites = hardNote.Composites.OfType<TextComposite>().ToList();

                    if (headerComposites.Any())
                    {
                        var queryHeaders = @"
                        INSERT INTO Composites(Id, Tag, Comment, Header, HardNoteId, CompositeType) 
                        VALUES (@Id, @Tag, @Comment, @Header, @HardNoteId, @CompositeType)";

                        await connection.ExecuteAsync(queryHeaders, headerComposites);
                    }

                    if (textComposites.Any())
                    {
                        var queryTexts = @"
                        INSERT INTO Composites(Id, Tag, Comment, Text, HardNoteId, CompositeType) 
                        VALUES (@Id, @Tag, @Comment, @Text, @HardNoteId, @CompositeType)";

                        await connection.ExecuteAsync(queryTexts, textComposites);
                    }
                }

                if (resultAddHardNote > 0) return true;
            }

            return false;
        }
        public IEnumerable<HardNote> Read()
        {
            throw new NotImplementedException();
        }
        public async Task<bool> Update(HardNote hardNote)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> Delete(string id)
        {
            throw new NotImplementedException();
        }
    }
}