using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Domain;

namespace PetFamily.Infrastructure.configurations;

public class PetConfiguration : IEntityTypeConfiguration<Pet> 
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {
        builder.ToTable("Pets");
        
        builder.Property(p => p.Id); // тут почитать про то как в бд запихивать данные
        
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100); // вынести константу 1:11:00
        
        //... все законфигурировать там через нейронк.
        
        // че то про конфигурацию valueObject на 1:05:00
        
        // builder.HasMany для один ко многим
    }
}