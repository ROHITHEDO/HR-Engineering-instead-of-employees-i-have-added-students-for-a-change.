using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagement.Infrastructure.Data.Configurations
{
    public class StudentDocumentConfiguration
      : IEntityTypeConfiguration<StudentDocument>
    {
        public void Configure(
            EntityTypeBuilder<StudentDocument> builder)
        {
            builder.HasKey(x => x.Id);

            builder
                .HasOne(x => x.Student)
                .WithMany(x => x.Documents)
                .HasForeignKey(x => x.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
