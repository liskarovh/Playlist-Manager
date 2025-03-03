namespace PlaylistManager.DAL.Entities;

public interface IEntity
{
    Guid Id { get; set; }
}


// Analogy:  CookBook        |       PlaylistManager
// --------------------------|----------------------------------
//           IEntity         |       IEntity
//           ---             |       MultimediaBaseEntity
//           IngredientEntity|       VideoMediaEntity
//           IngredientEntity|       AudioMediaEntity
//           IngredientAmount|       PlaylistMultimediaEntity
//           RecipeEntity    |       PlaylistEntity
