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

        // Первичный ключ
        builder.HasKey(v => v.Id);

        // 1. Value Object: FullName
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

        // 2. Value Object: Description
        builder.ComplexProperty(v => v.Description, dBuilder =>
        {
            dBuilder.Property(d => d.Value)
                .IsRequired()
                .HasMaxLength(Constants.MaxHighDescription)
                .HasColumnName("description");
        });

        // 3. Value Object: YearsOfExperience (PhysicalAttributes или ваш VO опыта)
        builder.ComplexProperty(v => v.YearsOfExperience, yBuilder =>
        {
            yBuilder.Property(y => y.Value)
                .IsRequired()
                .HasColumnName("years_of_experience");
        });

        // 4. Value Object: OwnerPhoneNumber
        builder.ComplexProperty(v => v.OwnerPhoneNumber, pBuilder =>
        {
            pBuilder.Property(p => p.Value)
                .IsRequired()
                .HasMaxLength(Constants.MaxLowTextLength)
                .HasColumnName("phone_number");
        });

        // 5. JSON Коллекция: DonationDetails
        builder.OwnsMany(v => v.DonationDetails, details =>
        {
            details.ToJson("donation_details");

            details.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(Constants.MaxLowTextLength);

            details.Property(d => d.Description)
                .IsRequired()
                .HasMaxLength(Constants.MaxLowTextLength);
        });

        // Информируем EF Core работать напрямую с приватным бэкинг-полем _donationDetails
        builder.Navigation(v => v.DonationDetails)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // 6. JSON Коллекция: SocialNetworks
        builder.OwnsMany(v => v.SocialNetworks, sn =>
        {
            sn.ToJson("social_networks");

            sn.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(Constants.MaxLowTextLength);

            sn.Property(s => s.Url)
                .IsRequired()
                .HasMaxLength(Constants.MaxMediumTextLength);
        });

        builder.Navigation(v => v.SocialNetworks)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // 7. Связь Один-ко-Многим: Volunteer -> Pets
        builder.HasMany(v => v.Pets)
            .WithOne()
            .HasForeignKey("volunteer_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(v => v.Pets)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}