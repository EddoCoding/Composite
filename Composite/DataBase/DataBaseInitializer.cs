using Dapper;
using System.Data;

namespace Composite.DataBase
{
    public class DataBaseInitializer
    {
        public static void Initialize(IDbConnection connection)
        {
            connection.Execute("Create Table If Not Exists HardNotes(Id Text Primary Key, Title Text Default '', DateCreate DateTime Not Null, Category Text, Password Text)");
            connection.Execute("Create Table If Not Exists CompositeBase(Id Text Primary Key, HardNoteId Text, ParentId Text, CompositeType Text, Tag Text, Comment Text, OrderIndex Integer, " +
                "Foreign Key(HardNoteId) References HardNotes(Id) On Delete Cascade, " +
                "Foreign Key(ParentId) References CompositeBase(Id) On Delete Cascade)");

            connection.Execute("Create Table If Not Exists TextComposites(Id Text Primary Key, Text Text, Foreign Key (Id) References CompositeBase(Id) On Delete Cascade)");
            connection.Execute("Create Table If Not Exists HeaderComposites(Id Text Primary Key, Text Text, FontWeight Text, FontSize Integer, Foreign Key (Id) References CompositeBase(Id) On Delete Cascade)");
            connection.Execute("Create Table If Not Exists QuoteComposites(Id Text Primary Key, Text Text, Foreign Key (Id) References CompositeBase(Id) On Delete Cascade)");
            connection.Execute("Create Table If Not Exists CodeComposites(Id Text Primary Key, Text Text, Foreign Key (Id) References CompositeBase(Id) On Delete Cascade)");
            connection.Execute("Create Table If Not Exists MarkerComposites(Id Text Primary Key, Text Text, Foreign Key (Id) References CompositeBase(Id) On Delete Cascade)");
            connection.Execute("Create Table If Not Exists NumericComposites(Id Text Primary Key, Number Integer, Text Text, Foreign Key (Id) References CompositeBase(Id) On Delete Cascade)");
            connection.Execute("Create Table If Not Exists TaskComposites(Id Text Primary Key, Completed Integer, Text Text, Foreign Key (Id) References CompositeBase(Id) On Delete Cascade)");
            connection.Execute("Create Table If Not Exists TasksComposites(Id Text Primary Key, Text Text, Completed Integer, Status Text, Foreign Key(Id) References CompositeBase(Id) On Delete Cascade)");
            connection.Execute("Create Table If Not Exists SubTaskComposites(Id Text Primary Key, Text Text, Completed Integer, Foreign Key(Id) References CompositeBase(Id) On Delete Cascade)");
            connection.Execute("Create Table If Not Exists LineComposites(Id Text Primary Key, LineSize Integer, LineColor Text, Foreign Key (Id) References CompositeBase(Id) On Delete Cascade)");
            connection.Execute("Create Table If Not Exists DocumentComposites(Id Text Primary Key, Text Text, Data Blob, Foreign Key (Id) References CompositeBase(Id) On Delete Cascade)");
            connection.Execute("Create Table If Not Exists DocumentsComposites(Id Text Primary Key, Text Text, Foreign Key (Id) References CompositeBase(Id) On Delete Cascade)");
            connection.Execute("Create Table If Not Exists DocumentMiniComposites(Id Text Primary Key, Text Text, Data Blob, Foreign Key (Id) References CompositeBase(Id) On Delete Cascade)");
            connection.Execute("Create Table If Not Exists ReferenceComposites(Id Text Primary Key, Text Text, ValueRef Text, Foreign Key (Id) References CompositeBase(Id) On Delete Cascade)");
            connection.Execute("Create Table If Not Exists ReferencesComposites(Id Text Primary Key, Foreign Key(Id) References CompositeBase(Id) On Delete Cascade)");
            connection.Execute("Create Table If Not Exists ImageComposites(Id Text Primary Key, HorizontalAlignment Text, Data Blob, Foreign Key (Id) References CompositeBase(Id) On Delete Cascade)");
            connection.Execute("Create Table If Not Exists FormattedTextComposites(Id Text Primary Key, BorderSize Integer, CornerRadius Integer, BorderColor Text, BackgroundColor Text, Data Blob, Foreign Key (Id) References CompositeBase(Id) On Delete Cascade)");
            connection.Execute("Create Table If Not Exists SongComposites(Id Text Primary Key, Title Text, Data Blob, Foreign Key (Id) References CompositeBase(Id) On Delete Cascade)");
            connection.Execute("Create Table If Not Exists SongsComposites(Id Text Primary Key, Text Text, Foreign Key(Id) References CompositeBase(Id) On Delete Cascade)");


            connection.Execute("Create Table If Not Exists Songs(Id Text Primary Key, Title Text, Data Blob)");
            connection.Execute("Create Table if Not Exists Categories(NameCategory Text Primary Key)");
            connection.Execute("Insert Or Ignore Into Categories(NameCategory) Values(@NameCategory)", new { NameCategory = "Uncategorized" });
        }
    }
}