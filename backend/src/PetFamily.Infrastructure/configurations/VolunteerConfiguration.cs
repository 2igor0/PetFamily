using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Domain.Shared;
using PetFamily.Domain.VolunteerManagement;

namespace PetFamily.Infrastructure.configurations;

public class VolunteerConfiguration : IEntityTypeConfiguration<Volunteer>
{
    public void Configure(EntityTypeBuilder<Volunteer> builder)
    {
        builder.ToTable("volunteers");

        builder.HasKey(volunteer => volunteer.Id);

        builder.ComplexProperty(volunteer => volunteer.FullName, fullName =>
        {
            fullName.Property(name => name.FirstName)
                .HasColumnName("first_name")
                .IsRequired()
                .HasMaxLength(Constants.MaxLowTextLength);

            fullName.Property(name => name.LastName)
                .HasColumnName("last_name")
                .IsRequired()
                .HasMaxLength(Constants.MaxLowTextLength);

            fullName.Property(name => name.Patronymic)
                .HasColumnName("patronymic")
                .HasMaxLength(Constants.MaxLowTextLength);
        });

        builder.ComplexProperty(volunteer => volunteer.Email, email =>
        {
            email.Property(value => value.Value)
                .HasColumnName("email")
                .IsRequired()
                .HasMaxLength(Constants.MaxEmailLength);
        });

        builder.ComplexProperty(volunteer => volunteer.Description, description =>
        {
            description.Property(value => value.Value)
                .HasColumnName("description")
                .IsRequired()
                .HasMaxLength(Constants.MaxHighDescription);
        });

        builder.ComplexProperty(volunteer => volunteer.YearsOfExperience, experience =>
        {
            experience.Property(value => value.Value)
                .HasColumnName("years_of_experience")
                .IsRequired();
        });

        builder.ComplexProperty(volunteer => volunteer.OwnerPhoneNumber, phoneNumber =>
        {
            phoneNumber.Property(value => value.Value)
                .HasColumnName("phone_number")
                .IsRequired()
                .HasMaxLength(Constants.MaxPhoneLength);
        });

        builder.Property(volunteer => volunteer.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(volunteer => volunteer.IsDeleted)
            .HasColumnName("is_deleted")
            .IsRequired();

        builder.Property(volunteer => volunteer.DeletedAt)
            .HasColumnName("deleted_at");

        builder.OwnsMany(volunteer => volunteer.SocialNetworks, socialNetworks =>
        {
            socialNetworks.ToTable("volunteer_social_networks");
            socialNetworks.WithOwner().HasForeignKey("volunteer_id");

            socialNetworks.Property(network => network.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(Constants.MaxLowTextLength);

            socialNetworks.Property(network => network.Url)
                .HasColumnName("url")
                .IsRequired()
                .HasMaxLength(Constants.MaxUrlLength);
        });

        builder.Navigation(volunteer => volunteer.SocialNetworks)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany(volunteer => volunteer.DonationDetails, donationDetails =>
        {
            donationDetails.ToTable("volunteer_donation_details");
            donationDetails.WithOwner().HasForeignKey("volunteer_id");

            donationDetails.Property(details => details.Name)
                .HasColumnName("name")
                .IsRequired()
                .HasMaxLength(Constants.MaxLowTextLength);

            donationDetails.Property(details => details.Description)
                .HasColumnName("description")
                .IsRequired()
                .HasMaxLength(Constants.MaxMediumTextLength);
        });

        builder.Navigation(volunteer => volunteer.DonationDetails)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(volunteer => volunteer.Pets)
            .WithOne()
            .HasForeignKey(pet => pet.VolunteerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(volunteer => volunteer.Pets)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(volunteer => volunteer.IsDeleted)
            .HasDatabaseName("ix_volunteers_is_deleted");

        builder.HasQueryFilter(volunteer => !volunteer.IsDeleted);
    }
}
