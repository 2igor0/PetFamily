using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Domain;
using PetFamily.Domain.Shared;

namespace PetFamily.Infrastructure.configurations;

public class PetConfiguration : IEntityTypeConfiguration<Pet> 
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {
        builder.ToTable("Pets");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(Constants.MaxLowTextLength); 
        
        builder.Property(p => p.Species)
            .IsRequired()
            .HasMaxLength(Constants.MaxLowTextLength);        
        
        builder.Property(p => p.Description).HasMaxLength(2000);
        
        builder.Property(p => p.Breed)
            .IsRequired()
            .HasMaxLength(Constants.MaxLowTextLength);

        builder.Property(p => p.Color)
            .IsRequired()
            .HasMaxLength(Constants.MaxLowTextLength);
        
        builder.Property(p => p.HealthInfo)
            .IsRequired()
            .HasMaxLength(Constants.MaxLowTextLength);
        
        builder.Property(p => p.CurrentAddress)
            .IsRequired()
            .HasMaxLength(Constants.MaxLowTextLength);
        
        builder.Property(p => p.Weight)
            .IsRequired();
        
        builder.Property(p => p.Height)
            .IsRequired();
        
        builder.Property(p => p.OwnerPhoneNumber)
            .IsRequired()
            .HasMaxLength(Constants.MaxLowTextLength);

        builder.Property(p => p.IsNeutered)
            .IsRequired();

        builder.Property(p => p.BirthDate);
        
        builder.Property(p => p.IsVaccinated)
            .IsRequired();

        builder.Property(p => p.HelpStatus)
            .IsRequired()
            .HasMaxLength(Constants.MaxLowTextLength);
        
        builder.OwnsMany(x => x.DonationDetails, details =>
        {
            details.ToJson(); 
            
            details.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(Constants.MaxLowTextLength);
            
            details.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(Constants.MaxLowTextLength);
        }).UsePropertyAccessMode(PropertyAccessMode.Field);;
        
        builder.Property(p => p.CreatedAt);
        
        builder.OwnsMany(x => x.Photos, details =>
        {
            details.ToJson();

            details.Property(c => c.IsMain)
                .IsRequired();
            
            details.Property(c => c.Path)
                .IsRequired()
                .HasMaxLength(Constants.MaxMediumTextLength);
        });
    }
}