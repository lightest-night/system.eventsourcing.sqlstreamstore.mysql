namespace LightestNight.System.EventSourcing.SqlStreamStore.MySql
{
    public class MySqlEventSourcingOptions : EventSourcingOptions
    {
        /// <summary>
        /// Whether to create the database schema if it doesn't already exist
        /// </summary>
        public bool CreateSchemaIfNotExists { get; set; } = true;
    }
}