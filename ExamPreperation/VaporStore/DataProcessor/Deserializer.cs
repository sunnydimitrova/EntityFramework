namespace VaporStore.DataProcessor
{
	using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.DataProcessor.Dto;

    public static class Deserializer
	{
		public static string ImportGames(VaporStoreDbContext context, string jsonString)
		{
			var sb = new StringBuilder();
			var gamesDto = JsonConvert.DeserializeObject<List<GamesImportDto>>(jsonString);

			var games = new List<Game>();

            foreach (var currentGame in gamesDto)
            {
                if (!IsValid(currentGame) || currentGame.Tags.Length < 1)
                {
					sb.AppendLine("Invalid Data");
					continue;
                }
				var gener = context.Genres.FirstOrDefault(x => x.Name == currentGame.Genre) ??
					new Genre { Name = currentGame.Genre };

				var developer = context.Developers.FirstOrDefault(x => x.Name == currentGame.Developer) ??
					new Developer { Name = currentGame.Developer };

				var game = new Game
				{
					Name = currentGame.Name,
					Price = currentGame.Price,
					ReleaseDate = currentGame.ReleaseDate.Value,
					Developer = developer,
					Genre = gener,					
				};

                foreach (var tagJson in currentGame.Tags)
                {
					var tag = context.Tags.FirstOrDefault(x => x.Name == tagJson) ??
						new Tag { Name = tagJson };
					game.GameTags.Add(new GameTag { Tag = tag });
                }

				context.Games.Add(game);
				context.SaveChanges();
				sb.AppendLine($"Added {currentGame.Name} ({currentGame.Genre}) with {currentGame.Tags.Length} tags");
            }

			return sb.ToString().TrimEnd();
		}

		public static string ImportUsers(VaporStoreDbContext context, string jsonString)
		{
			throw new NotImplementedException();
		}

		public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
		{
			throw new NotImplementedException();
		}

		private static bool IsValid(object dto)
		{
			var validationContext = new ValidationContext(dto);
			var validationResult = new List<ValidationResult>();

			return Validator.TryValidateObject(dto, validationContext, validationResult, true);
		}
	}
}