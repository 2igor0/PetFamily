using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Domain;
using PetFamily.Domain.Shared;

namespace PetFamily.Infrastructure.configurations;

public class VolunteerConfiguration : IEntityTypeConfiguration<Volunteer> 
{
    public void Configure(EntityTypeBuilder<Volunteer> builder)
    {
        builder.ToTable("volunteers");

        builder.HasKey(v => v.Id);

        // Маппинг Record FullName (или OwnsOne, если используете EF Core 7)
        builder.ComplexProperty(v => v.FullName, fnBuilder =>
        {
            fnBuilder.Property(fn => fn.FirstName)
                .IsRequired()
                .HasMaxLength(Constants.MaxLowTextLength)
                .HasColumnName("first_name");

            fnBuilder.Property(fn => fn.LastName)
                .IsRequired()
                .HasMaxLength(Constants.MaxLowTextLength)
                .HasColumnName("last_name");

            fnBuilder.Property(fn => fn.Patronymic)
                .IsRequired(false)
                .HasMaxLength(Constants.MaxLowTextLength)
                .HasColumnName("patronymic");
        });

        builder.Property(v => v.Description)
            .IsRequired()
            .HasMaxLength(Constants.MaxHighDescription);

        builder.Property(v => v.YearsOfExperience)
            .IsRequired();

        // Поле int (в идеале переделать на string, но конфигурируем как есть в модели)
        builder.Property(v => v.OwnerPhoneNumber)
            .IsRequired();
        
        builder.OwnsMany(v => v.DonationDetails, details =>
        {
            details.ToJson();
            
            details.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(Constants.MaxLowTextLength);
            
            details.Property(d => d.Description)
                .IsRequired()
                .HasMaxLength(Constants.MaxLowTextLength);
        })
        .UsePropertyAccessMode(PropertyAccessMode.Field);
        
        builder.OwnsMany(v => v.SocialNetworks, sn =>
        {
            sn.ToJson();

            sn.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(Constants.MaxLowTextLength);

            sn.Property(s => s.Url)
                .IsRequired()
                .HasMaxLength(Constants.MaxMediumTextLength);
        })
        .UsePropertyAccessMode(PropertyAccessMode.Field);
        
        builder.HasMany(v => v.Pets) // че это)
            .WithOne()
            .HasForeignKey("volunteer_id")
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Navigation(v => v.Pets) // че это))
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
    
} 