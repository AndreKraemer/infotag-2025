using DontLetMeExpireAvalonia.Models;
using System;
using System.Collections.Generic;
using System.Threading;

namespace DontLetMeExpireAvalonia.Services;

public static class DummyData
{

    public static List<StorageLocation> Locations { get; } =
    [
        new StorageLocation
        {
            Id = "f764f62e-d61c-48d4-ac2a-bcd8e1dce89e", Name = GetTranslation("Freezer", "Gefrierschrank"),
            Icon = "M440-80v-166L310-118l-56-56 186-186v-80h-80L174-254l-56-56 128-130H80v-80h166L118-650l56-56 186 186h80v-80L254-786l56-56 130 128v-166h80v166l130-128 56 56-186 186v80h80l186-186 56 56-128 130h166v80H714l128 130-56 56-186-186h-80v80l186 186-56 56-130-128v166h-80Z"
        },
        new StorageLocation
        {
            Id = "1ce2ed73-2353-496a-8381-82ac580dc16b", Name = GetTranslation("Refrigerator", "Kühlschrank"),
            Icon = "M320-640v-120h80v120h-80Zm0 360v-200h80v200h-80ZM240-80q-33 0-56.5-23.5T160-160v-640q0-33 23.5-56.5T240-880h480q33 0 56.5 23.5T800-800v640q0 33-23.5 56.5T720-80H240Zm0-80h480v-360H240v360Zm0-440h480v-200H240v200Z"
        },
        new StorageLocation
        {
            Id = "fc01111d-1e47-406a-b695-04cf7bf4f521", Name = GetTranslation("Pantry", "Vorratsschrank"),
            Icon = "M120-40v-880h80v80h560v-80h80v880h-80v-80H200v80h-80Zm80-480h80v-160h240v160h240v-240H200v240Zm0 320h240v-160h240v160h80v-240H200v240Zm160-320h80v-80h-80v80Zm160 320h80v-80h-80v80ZM360-520h80-80Zm160 320h80-80Z"
        }
    ];

    public static List<Item> Items { get; } =
    [
        new Item
        {
            Id = "ea0b468a-220c-470f-aff6-3ebf7eadbfce",
            Name = GetTranslation("Leaf spinach 1lb (frozen)", "Blattspinat 500g (TK)"),
            ExpirationDate = DateTime.Today,
            StorageLocationId = Locations[0].Id,
            Amount = 1
        },
        new Item
        {
            Id = "a19d6623-f1e6-4b61-94ef-d7c620872ecd",
            Name = GetTranslation("Garden peas 1lb (frozen)", "Gartenerbsen 400g (TK)"),
            ExpirationDate = DateTime.Today.AddMonths(3).AddDays(5),
            StorageLocationId = Locations[0].Id,
            Amount = 1
        },
        new Item
        {
            Id = "29374ad8-3614-497c-891f-5202ebafcf6f",
            Name = GetTranslation("Vegetable stir fry 2lb (frozen)", "Gemüsepfanne 750g (TK)"),
            ExpirationDate = DateTime.Today.AddMonths(9).AddDays(3),
            StorageLocationId = Locations[0].Id,
            Amount = 1
        },
        new Item
        {
            Id = "d5c9b410-af26-4ebb-b074-1317b2399065",
            Name = GetTranslation("Mixed vegetables 1lb (frozen)", "Kaisergemüse 450g (TK)"),
            ExpirationDate = DateTime.Today.AddDays(4),
            StorageLocationId = Locations[0].Id,
            Amount = 2
        },
        new Item
        {
            Id = "1ce2ed73-2353-496a-8381-82ac580dc16b",
            Name = GetTranslation("Orange juice", "Orangensaft"),
            ExpirationDate = DateTime.Today.AddDays(3),
            StorageLocationId = Locations[1].Id,
            Amount = 5
        },
        new Item
        {
            Id = "12f0509b-567d-4983-ae81-fc5fb56e4117",
            Name = GetTranslation("Broccoli", "Brokkoli"),
            ExpirationDate = DateTime.Today.AddDays(6),
            StorageLocationId = Locations[1].Id,
            Amount = 2
        },
        new Item
        {
            Id = "a319b326-9fe3-4b70-ab26-bb66fe749c14",
            Name = GetTranslation("Vegetable broth", "Gemüsebrühe"),
            ExpirationDate = DateTime.Today.AddDays(-5),
            StorageLocationId = Locations[2].Id,
            Amount = 1
        },
        new Item
        {
            Id = "d1a7396a-9198-46ee-b541-f0e180911b7b",
            Name = GetTranslation("Rice", "Reis"),
            ExpirationDate = DateTime.Today.AddDays(-25),
            StorageLocationId = Locations[2].Id,
            Amount = 1
        }
    ];

    private static string GetTranslation(string english, string german)
    {
        return Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == "de" ? german : english;
    }
}