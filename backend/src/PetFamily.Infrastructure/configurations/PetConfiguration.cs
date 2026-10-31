using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Domain.PetManagement;
using PetFamily.Domain.PetManagement.Enums;
using PetFamily.Domain.PetManagement.ValueObjects;
using PetFamily.Domain.Shared;

namespace PetFamily.Infrastructure.configurations;

public class PetConfiguration : IEntityTypeConfiguration<Pet>
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {
        builder.ToTable("pets");

        builder.HasKey(pet => pet.Id);

        builder.Property(pet => pet.VolunteerId)
            .HasColumnName("volunteer_id")
            .IsRequired();

        builder.ComplexProperty(pet => pet.Name, name =>
        {
            name.Property(value => value.Value)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(Constants.MaxLowTextLength);
        });

        builder.ComplexProperty(pet => pet.SpeciesBreed, speciesBreed =>
        {
            speciesBreed.Property(value => value.SpeciesId)
                .HasColumnName("species_id")
                .IsRequired();

            speciesBreed.Property(value => value.BreedId)
                .HasColumnName("breed_id")
                .IsRequired();
        });

        builder.ComplexProperty(pet => pet.Description, description =>
        {
            description.Property(value => value.Value)
                .HasColumnName("description")
                .IsRequired()
                .HasMaxLength(Constants.MaxHighDescription);
        });

        builder.ComplexProperty(pet => pet.Color, color =>
        {
            color.Property(value => value.Value)
                .HasColumnName("color")
                .IsRequired()
                .HasMaxLength(Constants.MaxLowTextLength);
        });

        builder.ComplexProperty(pet => pet.HealthInfoDescription, health =>
        {
            health.Property(value => value.Value)
                .HasColumnName("health_info_description")
                .IsRequired()
                .HasMaxLength(Constants.MaxHighDescription);
        });

        builder.ComplexProperty(pet => pet.CurrentAddress, address =>
        {
            address.Property(value => value.Country)
                .HasColumnName("country")
                .IsRequired()
                .HasMaxLength(Constants.MaxLowTextLength);

            address.Property(value => value.City)
                .HasColumnName("city")
                .IsRequired()
                .HasMaxLength(Constants.MaxLowTextLength);

            address.Property(value => value.Street)
                .HasColumnName("street")
                .IsRequired()
                .HasMaxLength(Constants.MaxLowTextLength);

            address.Property(value => value.HouseNumber)
                .HasColumnName("house_number")
                .IsRequired();
        });

        builder.ComplexProperty(pet => pet.Weight, weight =>
        {
            weight.Property(value => value.Value)
                .HasColumnName("weight")
                .IsRequired();
        });

        builder.ComplexProperty(pet => pet.Height, height =>
        {
            height.Property(value => value.Value)
                .HasColumnName("height")
                .IsRequired();
        });

        builder.ComplexProperty(pet => pet.OwnerPhoneNumber, phoneNumber =>
        {
            phoneNumber.Property(value => value.Value)
                .HasColumnName("owner_phone_number")
                .IsRequired()
                .HasMaxLength(Constants.MaxPhoneLength);
        });

        builder.Property(pet => pet.IsNeutered)
            .HasColumnName("is_neutered")
            .IsRequired();

        builder.Property(pet => pet.BirthDate)
            .HasColumnName("birth_date")
            .IsRequired();

        builder.Property(pet => pet.IsVaccinated)
            .HasColumnName("is_vaccinated")
            .IsRequired();

        builder.Property(pet => pet.HelpStatus)
            .HasColumnName("help_status")
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(Constants.MaxLowTextLength);

        builder.Property(pet => pet.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(pet => pet.IsDeleted)
            .HasColumnName("is_deleted")
            .IsRequired();

        builder.Property(pet => pet.DeletedAt)
            .HasColumnName("deleted_at");

        builder.OwnsMany(pet => pet.Photos, photos =>
        {
            photos.ToTable("pet_photos");
            photos.WithOwner().HasForeignKey("pet_id");

            photos.Property(photo => photo.Path)
                .HasColumnName("path")
                .IsRequired()
                .HasMaxLength(Constants.MaxMediumTextLength);

            photos.Property(photo => photo.IsMain)
                .HasColumnName("is_main")
                .IsRequired();
        });

        builder.Navigation(pet => pet.Photos)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany(pet => pet.DonationDetails, details =>
        {
            details.ToTable("pet_donation_details");
            details.WithOwner().HasForeignKey("pet_id");

            details.Property(donation => donation.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(Constants.MaxLowTextLength);

            details.Property(donation => donation.Description)
                .HasColumnName("description")
                .IsRequired()
                .HasMaxLength(Constants.MaxMediumTextLength);
        });

        builder.Navigation(pet => pet.DonationDetails)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(pet => pet.VolunteerId)
            .HasDatabaseName("ix_pets_volunteer_id");

        builder.HasIndex(pet => pet.IsDeleted)
            .HasDatabaseName("ix_pets_is_deleted");

        builder.HasIndex(pet => pet.HelpStatus)
            .HasDatabaseName("ix_pets_help_status");

        builder.HasQueryFilter(pet => !pet.IsDeleted);
    }
}
