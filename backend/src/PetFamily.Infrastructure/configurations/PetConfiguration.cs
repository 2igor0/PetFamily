using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Domain;
using PetFamily.Domain.PetManagement;
using PetFamily.Domain.Shared;

namespace PetFamily.Infrastructure.configurations;

public class PetConfiguration : IEntityTypeConfiguration<Pet>  // заново отрефакторить
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {
        builder.ToTable("Pets");
        
        builder.HasKey(p => p.Id);

        builder.ComplexProperty(p => p.Name, title =>
        {
            title.Property(x => x.Value)
                .IsRequired()
                .HasMaxLength(Constants.MaxLowTextLength);
        }); 
        
        /*builder.Property(p => p.Species)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(Constants.MaxLowTextLength);   */     
        
        builder.ComplexProperty(p => p.Description, des =>
        {
            des.Property(x => x.Value)
                .HasMaxLength(Constants.MaxLowTextLength);
        }); 
        
        /*builder.ComplexProperty(p => p.Breed, breed =>
        {
            breed.Property(x => x.Value)
                .IsRequired()
                .HasMaxLength(Constants.MaxLowTextLength);
        }); */
        
        
        builder.ComplexProperty(p => p.Color, color =>
        {
            color.Property(x => x.Value)
                .HasMaxLength(Constants.MaxLowTextLength);
        }); 
        
        builder.ComplexProperty(p => p.HealthInfoDescription, health =>
        {
            health.Property(x => x.Value)
                .IsRequired()
                .HasMaxLength(Constants.MaxLowTextLength);
        });

        builder.ComplexProperty(p => p.CurrentAddress, address =>
        {
            address.ToJson();

            address.Property(x => x.Country).IsRequired().HasMaxLength(Constants.MaxLowTextLength);
            address.Property(x => x.City).IsRequired().HasMaxLength(Constants.MaxLowTextLength);
            address.Property(x => x.Street).IsRequired().HasMaxLength(Constants.MaxLowTextLength);
            address.Property(x => x.HouseNumber).IsRequired();
        });
        
        builder.ComplexProperty(p => p.Weight, weight =>
        {
            weight.Property(x => x.Value)
                .IsRequired();
        });
        
        builder.ComplexProperty(p => p.Height, height =>
        {
            height.Property(x => x.Value)
                .IsRequired();
        });
        
        builder.ComplexProperty(p => p.OwnerPhoneNumber, phoneNumber =>
        {
            phoneNumber.Property(x => x.Value)
                .IsRequired()
                .HasMaxLength(Constants.MaxLowTextLength);
        });

        builder.Property(p => p.IsNeutered)
            .IsRequired();
        
        builder.ComplexProperty(p => p.BirthDate, date =>
        {
            date.ToJson();

            date.Property(p => p.Date);
            date.Property(p => p.Day);
            date.Property(p => p.Month);
            date.Property(p => p.Year);
        });
        
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
        
        builder.ComplexProperty(p => p.CreatedAt, date =>
        {
            date.ToJson();

            date.Property(p => p.Date);
            date.Property(p => p.Day);
            date.Property(p => p.Month);
            date.Property(p => p.Year);
        });
        
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