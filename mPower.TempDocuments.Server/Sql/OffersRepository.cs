using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using mPower.Framework;
using mPower.Framework.Geo;
using mPower.TempDocuments.Server.DocumentServices;
using mPower.TempDocuments.Server.Documents;

namespace mPower.TempDocuments.Server.Sql
{
    public class OffersRepository : IOffersRepository
    {
        private readonly string _connectionString;

        public OffersRepository(MPowerSettings  settings)
        {
            _connectionString = settings.SqlTempDatabase;
        }

        public IEnumerable<OfferEntity> GetNear(OfferFilter filter)
        {
            filter.PagingInfo.TotalCount = 1000;
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var cmd = connection.CreateCommand())
                {
                    connection.Open();
                    cmd.CommandText = "GetNear";
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (filter.SearchTerm.HasValue())
                    {
                        cmd.Parameters.AddWithValue("@search", filter.SearchTerm);
                    }
                    if (filter.CategoryNameIn != null && filter.CategoryNameIn.Any())
                    {
                        for (int i = 0; i < filter.CategoryNameIn.Count() && i < 5; i++)
                        {
                            cmd.Parameters.AddWithValue("@category" + (i + 1), filter.CategoryNameIn[i]);
                        }
                    }
                    cmd.Parameters.AddWithValue("@skip", filter.PagingInfo.Skip);
                    cmd.Parameters.AddWithValue("@take", filter.PagingInfo.Take);
                    cmd.Parameters.AddWithValue("@radius", (filter.Radius ?? Location.EarthRaduisInMiles));
                    cmd.Parameters.AddWithValue("@latitude", filter.GeoLocation.Value.Latitude);
                    cmd.Parameters.AddWithValue("@longitude", filter.GeoLocation.Value.Longitude);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                            var logoOrdinal = reader.GetOrdinal("Logo");
                            var item = new OfferEntity
                            {
                                Id = reader.GetString(reader.GetOrdinal("Id")),
                                Distance = reader.GetDouble(reader.GetOrdinal("Distance")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Merchant = reader.GetString(reader.GetOrdinal("Merchant")),
                                Category = reader.GetString(reader.GetOrdinal("Category")),
                                Logo = reader.IsDBNull(logoOrdinal) ? null : reader.GetString(logoOrdinal),
                                FormatedAward = reader.GetString(reader.GetOrdinal("Award")),
                                Location =
                                    new Location(reader.GetDouble(reader.GetOrdinal("Latitude")),
                                                 reader.GetDouble(reader.GetOrdinal("Longitude")))
                            };
                        yield return item;
                    }
                }
            }
        }

        private T Execute<T>(OfferFilter filter, string procedureName, Func<SqlCommand,T> result)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var cmd = connection.CreateCommand())
                {
                    connection.Open();
                    cmd.CommandText = procedureName;
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (filter.SearchTerm.HasValue())
                    {
                        cmd.Parameters.AddWithValue("@search", filter.SearchTerm);
                    }
                    if (filter.CategoryNameIn != null && filter.CategoryNameIn.Any())
                    {
                        for (int i = 0; i < filter.CategoryNameIn.Count() && i < 5; i++)
                        {
                            cmd.Parameters.AddWithValue("@category" + (i + 1), filter.CategoryNameIn[i]);
                        }
                    }
                    cmd.Parameters.AddWithValue("@skip", filter.PagingInfo.Skip);
                    cmd.Parameters.AddWithValue("@take", filter.PagingInfo.Take);
                    cmd.Parameters.AddWithValue("@radius", (filter.Radius ?? Location.EarthRaduisInMiles));
                    cmd.Parameters.AddWithValue("@latitude", filter.GeoLocation.Value.Latitude);
                    cmd.Parameters.AddWithValue("@longitude", filter.GeoLocation.Value.Longitude);
                    return result(cmd);
                }
            }
        }

        public int GetNearCount(OfferFilter filter)
        {
            return Execute(filter, "GetNearCount", (cmd) => (int) cmd.ExecuteScalar());
        }

        public void Insert(OfferDocument[] batch)
        {
            foreach (var doc in batch)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    using (var cmd = connection.CreateCommand())
                    {
                        connection.Open();
                        cmd.CommandText = "CreateOffer";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", doc.Id);
                        cmd.Parameters.AddWithValue("@title", doc.Title);
                        cmd.Parameters.AddWithValue("@merchant", doc.Merchant);
                        cmd.Parameters.AddWithValue("@category", doc.CategoryName);
                        cmd.Parameters.AddWithValue("@logo", doc.MerchantLogoImage);
                        cmd.Parameters.AddWithValue("@award", doc.GetFormatedAward());
                        cmd.Parameters.AddWithValue("@latitude", (float)doc.GeoLocation.Latitude);
                        cmd.Parameters.AddWithValue("@longitude", (float)doc.GeoLocation.Longitude);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        } 

        public void RemoveAll()
        {
            DropAndCreateTable();
            DropAndCreateIndexes();
            UpdateCreateOfferProcedure();
            UpdateGetNearProcedure(); 
            UpdateGetNearCountProcedure();
        }

        private void DropAndCreateIndexes()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var cmd = connection.CreateCommand())
                {
                    connection.Open();
                    cmd.CommandText = @"CREATE NONCLUSTERED INDEX [CategoryIndex] ON [dbo].[Offers] 
(
	[Category] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
";
                    cmd.ExecuteNonQuery();
                }
            }
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var cmd = connection.CreateCommand())
                {
                    connection.Open();
                    cmd.CommandText = @"CREATE NONCLUSTERED INDEX [MerchantIndex] ON [dbo].[Offers] 
(
	[Merchant] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]";
                    cmd.ExecuteNonQuery();
                }
            }
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var cmd = connection.CreateCommand())
                {
                    connection.Open();
                    cmd.CommandText = @"CREATE SPATIAL INDEX [GeoIndex] ON [dbo].[Offers] 
(
	[Point]
)USING  GEOGRAPHY_GRID 
WITH (
GRIDS =(LEVEL_1 = MEDIUM,LEVEL_2 = MEDIUM,LEVEL_3 = MEDIUM,LEVEL_4 = MEDIUM), 
CELLS_PER_OBJECT = 16, PAD_INDEX  = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void DropAndCreateTable()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var cmd = connection.CreateCommand())
                {
                    connection.Open();
                    cmd.CommandText = @"
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Offers]') AND type in (N'U'))
DROP TABLE [dbo].[Offers]";
                    cmd.ExecuteNonQuery();
                }
            }
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var cmd = connection.CreateCommand())
                {
                    connection.Open();
                    cmd.CommandText = @"CREATE TABLE [dbo].[Offers](
	[Id] [nvarchar](50) NOT NULL,
	[Merchant] [nvarchar](200) NOT NULL,
	[Title] [nvarchar](4000) NOT NULL,
	[Point] [geography] NOT NULL,
	[Category] [nvarchar](200) NOT NULL,
	[Logo] [nvarchar](200) NULL,
	[Award] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Offer] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void UpdateCreateOfferProcedure()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var cmd = connection.CreateCommand())
                {
                    connection.Open();
                    cmd.CommandText = @"
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CreateOffer]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[CreateOffer]";
                    cmd.ExecuteNonQuery();
                }
            }
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var cmd = connection.CreateCommand())
                {
                    connection.Open();
                    cmd.CommandText = @"CREATE PROCEDURE [dbo].[CreateOffer]
	
	(
	@id nvarchar(50),
	@merchant nvarchar(200),
	@category nvarchar(200),
	@logo nvarchar(200) = NULL,
	@award nvarchar(50),
	@title nvarchar(4000),
	@latitude float,
	@longitude float
	)
	
AS
IF NOT EXISTS ( SELECT 1 FROM Offers WHERE Id = @id )
BEGIN
 DECLARE @point GEOGRAPHY
		SET @point = geography::STGeomFromText('POINT(' + CAST(@longitude AS varchar(50)) + ' '  +  CAST(@latitude AS varchar(50)) + ')', 4326);
INSERT INTO Offers
VALUES ( @id  ,@merchant, @title, @point, @category, @logo, @award)
END
	RETURN";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void UpdateGetNearProcedure()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var cmd = connection.CreateCommand())
                {
                    connection.Open();
                    cmd.CommandText = @"
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetNear]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetNear]";
                    cmd.ExecuteNonQuery();
                }
            }
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var cmd = connection.CreateCommand())
                {
                    connection.Open();
                    cmd.CommandText = @"
CREATE PROCEDURE [dbo].[GetNear]
(
@latitude float,
@longitude float,
@radius float,
@skip int,
@take int,
@search nvarchar(100) = NULL,
@category1 nvarchar(200) = NULL,
@category2 nvarchar(200) = NULL,
@category3 nvarchar(200) = NULL,
@category4 nvarchar(200) = NULL,
@category5 nvarchar(200) = NULL
)
AS	

DECLARE @point GEOGRAPHY
SET @point = geography::STGeomFromText('POINT(' + CAST(@longitude AS varchar(50)) + ' '  +  CAST(@latitude AS varchar(50)) + ')',4326);

SELECT TOP(@take) Id, Merchant, Title, Distance, Category, Logo, Award, w.Point.Lat as Latitude, w.Point.Long as Longitude
FROM (SELECT *, ROW_NUMBER() OVER (ORDER BY a.Distance) Rownum FROM
(SELECT *, 
MIN(t.Distance) OVER(PARTITION BY t.Merchant, t.Title) AS MinDistance
FROM (SELECT *, (s.Point.STDistance(@point)/1000.0/1.61185124) as Distance
		FROM [Offers] s 
		WHERE
		 (@search IS NULL OR (lower(s.Title) like '%' + @search + '%' OR lower(s.Merchant) like '%' + @search + '%')) 
		 AND ( (@category1 IS NULL AND @category2 IS NULL AND @category3 IS NULL AND @category4 IS NULL AND @category5 IS NULL )
		 OR (@category1 IS NOT NULL AND s.Category = @category1) 
		 OR (@category2 IS NOT NULL AND s.Category = @category2) 
		 OR (@category3 IS NOT NULL AND s.Category = @category3) 
		 OR (@category4 IS NOT NULL AND s.Category = @category4) 
		 OR (@category5 IS NOT NULL AND s.Category = @category5)) 
		 AND s.Point.STDistance(@point) <= @radius*1000.0*1.61185124
		) t) a
WHERE a.Distance = a.MinDistance) w WHERE w.Rownum > @skip";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void UpdateGetNearCountProcedure()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var cmd = connection.CreateCommand())
                {
                    connection.Open();
                    cmd.CommandText = @"
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetNearCount]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetNearCount]";
                    cmd.ExecuteNonQuery();
                }
            }
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var cmd = connection.CreateCommand())
                {
                    connection.Open();
                    cmd.CommandText = @"
CREATE PROCEDURE [dbo].[GetNearCount]
(
@latitude float,
@longitude float,
@radius float,
@skip int,
@take int,
@search nvarchar(100) = NULL,
@category1 nvarchar(200) = NULL,
@category2 nvarchar(200) = NULL,
@category3 nvarchar(200) = NULL,
@category4 nvarchar(200) = NULL,
@category5 nvarchar(200) = NULL
)
AS	

DECLARE @point GEOGRAPHY
SET @point = geography::STGeomFromText('POINT(' + CAST(@longitude AS varchar(50)) + ' '  +  CAST(@latitude AS varchar(50)) + ')',4326);

(SELECT COUNT(*) FROM
(SELECT *, 
MIN(t.Distance) OVER(PARTITION BY t.Merchant, t.Title) AS MinDistance
FROM (SELECT *, (s.Point.STDistance(@point)/1000.0/1.61185124) as Distance
		FROM [Offers] s 
		WHERE
		 (@search IS NULL OR (lower(s.Title) like '%' + @search + '%' OR lower(s.Merchant) like '%' + @search + '%')) 
		 AND ( (@category1 IS NULL AND @category2 IS NULL AND @category3 IS NULL AND @category4 IS NULL AND @category5 IS NULL )
		 OR (@category1 IS NOT NULL AND s.Category = @category1) 
		 OR (@category2 IS NOT NULL AND s.Category = @category2) 
		 OR (@category3 IS NOT NULL AND s.Category = @category3) 
		 OR (@category4 IS NOT NULL AND s.Category = @category4) 
		 OR (@category5 IS NOT NULL AND s.Category = @category5)) 
		 AND s.Point.STDistance(@point) <= @radius*1000.0*1.61185124
		) t) a
WHERE a.Distance = a.MinDistance)";
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
} 