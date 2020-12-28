using ExoticRentals.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExoticRentals.Api.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens")
                .HasKey(t => t.Key);

            builder.Property(t => t.Key)
                .ValueGeneratedOnAdd();

            builder.HasOne(t => t.User)
                .WithMany(t => t.RefreshTokens)
                .HasForeignKey(t => t.UserId);

        }
    }
}
