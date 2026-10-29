using Microsoft.Extensions.Configuration;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using StudentManagement.Application.DTOs.Search;
using StudentManagement.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement.Infrastructure.Search
{
    public class QdrantVectorSearchService : IVectorSearchService
    {
        private const int VectorSize = 768;

        private readonly QdrantClient _client;
        private readonly IEmbeddingService _embeddingService;
        private readonly IConfiguration _configuration;

        public QdrantVectorSearchService(
            IEmbeddingService embeddingService,
            IConfiguration configuration)
        {
            _embeddingService = embeddingService;
            _configuration = configuration;

            var host =
                _configuration["Qdrant:Host"] ?? "localhost";

            var port =
                int.Parse(
                    _configuration["Qdrant:GrpcPort"] ?? "6334");

            _client = new QdrantClient(
                host: host,
                port: port);
        }

        public async Task IndexAsync(
            string text,
            int studentId,
            int documentId,
            int chunkId)
        {
            var collectionName =
                GetCollectionName();

            await EnsureCollectionExistsAsync(collectionName);

            var embedding =
                await _embeddingService
                    .GenerateEmbeddingAsync(text);

            var pointId =
                ((ulong)documentId << 32)
                | (uint)chunkId;

            var point = new PointStruct
            {
                Id = pointId,

                Vectors = embedding,

                Payload =
            {
                ["student_id"] = studentId,
                ["document_id"] = documentId,
                ["chunk_id"] = chunkId,
                ["text"] = text
            }
            };

            await _client.UpsertAsync(
                collectionName,
                new[] { point });
        }

        public async Task<List<VectorSearchResult>> SearchAsync(
            string query,
            int studentId,
            int topK = 5)
        {
            var collectionName =
                GetCollectionName();

            await EnsureCollectionExistsAsync(collectionName);

            var embedding =
                await _embeddingService
                    .GenerateEmbeddingAsync(query);

            var results =
                await _client.SearchAsync(
                    collectionName,
                    embedding,
                    limit: (ulong)topK,
                    filter: new Filter
                    {
                        Must =
                        {
                        new Condition
                        {
                            Field =
                                new FieldCondition
                                {
                                    Key = "student_id",

                                    Match =
                                        new Match
                                        {
                                            Integer = studentId
                                        }
                                }
                        }
                        }
                    });

            return results.Select(x =>
                new VectorSearchResult
                {
                    Text = x.Payload["text"].StringValue,

                    StudentId =
                        (int)x.Payload["student_id"].IntegerValue,

                                DocumentId =
                        (int)x.Payload["document_id"].IntegerValue,

                                ChunkId =
                        (int)x.Payload["chunk_id"].IntegerValue,

                    Score = x.Score
                })
                .ToList();
        }

        private async Task EnsureCollectionExistsAsync(
            string collectionName)
        {
            var exists =
                await _client.CollectionExistsAsync(
                    collectionName);

            if (exists)
                return;

            await _client.CreateCollectionAsync(
                collectionName,
                new VectorParams
                {
                    Size = VectorSize,
                    Distance = Distance.Cosine
                });
        }

        private string GetCollectionName()
        {
            return _configuration["Qdrant:CollectionName"]
                ?? "student_documents";
        }
    }
    //public class QdrantVectorSearchService : IVectorSearchService
    //{
    //    private const string CollectionName = "student_documents";
    //    private const int VectorSize = 768;

    //    private readonly QdrantClient _client;
    //    private readonly IEmbeddingService _embeddingService;

    //    public QdrantVectorSearchService(
    //        IEmbeddingService embeddingService)
    //    {
    //        _client = new QdrantClient(
    //            host: "localhost",
    //            port: 6334);

    //        _embeddingService = embeddingService;
    //    }

    //    public async Task IndexAsync(
    //        string text,
    //        int studentId,
    //        int documentId,
    //        int chunkId)
    //    {
    //        await EnsureCollectionExistsAsync();

    //        var embedding =
    //            await _embeddingService.GenerateEmbeddingAsync(text);

    //        var pointId = Guid.NewGuid();

    //        var point = new PointStruct
    //        {
    //            Id = pointId,
    //            Vectors = embedding,
    //            Payload =
    //        {
    //            ["student_id"] = studentId,
    //            ["document_id"] = documentId,
    //            ["chunk_id"] = chunkId,
    //            ["text"] = text
    //        }
    //        };

    //        await _client.UpsertAsync(
    //            collectionName: CollectionName,
    //            points: new[] { point });
    //    }

    //    public async Task<List<string>> SearchAsync(
    //        string query,
    //        int studentId,
    //        int topK = 5)
    //    {
    //        await EnsureCollectionExistsAsync();

    //        var embedding =
    //            await _embeddingService.GenerateEmbeddingAsync(query);

    //        var results = await _client.SearchAsync(
    //            collectionName: CollectionName,
    //            vector: embedding,
    //            limit: (ulong)topK,
    //            filter: new Filter
    //            {
    //                Must =
    //                {
    //                new Condition
    //                {
    //                    Field =
    //                    new FieldCondition
    //                    {
    //                        Key = "student_id",
    //                        Match = new Match
    //                        {
    //                            Integer = studentId
    //                        }
    //                    }
    //                }
    //                }
    //            });

    //        return results
    //            .Select(x => x.Payload["text"].StringValue)
    //            .ToList();
    //    }

    //    private async Task EnsureCollectionExistsAsync()
    //    {
    //        var exists =
    //            await _client.CollectionExistsAsync(CollectionName);

    //        if (exists)
    //            return;

    //        await _client.CreateCollectionAsync(
    //            collectionName: CollectionName,
    //            vectorsConfig: new VectorParams
    //            {
    //                Size = VectorSize,
    //                Distance = Distance.Cosine
    //            });
    //    }
    //}
}
