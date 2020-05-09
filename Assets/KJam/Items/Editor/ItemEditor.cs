using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class ItemEditor
{
	public static string ASSET_FOLDER = "Assets/KJam/Items/Resources/Items";
	public static string[] ASSET_SEARCH_FOLDERS = new string[] {
			ASSET_FOLDER,
		};

	[MenuItem( "Assets/Create/KJam - Item", false, 1 )]
	public static void create_asset()
	{
		BaseItem asset = (BaseItem)ScriptableObject.CreateInstance(typeof(BaseItem));
		asset.name = "NewITEM";

		string asset_path = ASSET_FOLDER + "/" + asset.name + ".asset";
		string unique_path = AssetDatabase.GenerateUniqueAssetPath(asset_path);
		AssetDatabase.CreateAsset( asset, unique_path );
		AssetDatabase.SaveAssets();

		//EditorUtility.FocusProjectWindow();
		//Selection.activeObject = asset;
	}

	[MenuItem( "Assets/KJam - Generate Icons", false, 1 )]
	public static void generate_sprites()
	{
		// Get all resoures of type item
		var items = Resources.LoadAll<BaseItem>( "Items" );
		foreach ( var item in items )
		{
			var tex = AssetPreview.GetAssetPreview( item.Prefab );
			//var sprite = Sprite.Create( tex, new Rect( 0, 0, tex.width, tex.height ), Vector2.zero );

			string path = ASSET_FOLDER + "/Sprites/" + item.name + ".png";

			byte[] _bytes = tex.EncodeToPNG();
			System.IO.File.WriteAllBytes( path, _bytes );

			//AssetDatabase.CreateAsset( tex, path );
			//Debug.Log( AssetDatabase.GetAssetPath( tex ) );
			//Texture2D asset = AssetDatabase.LoadAssetAtPath( path, typeof( Texture2D ) );
			//asset.
		}
	}

	public static void draw_class_gui( SerializedProperty sp )
	{
		SerializedProperty end_sp = sp.GetEndProperty();

		SerializedProperty it = sp.Copy();
		it.Next( true );
		while ( true )
		{
			EditorGUILayout.PropertyField( it );

			bool next = it.NextVisible(false);
			if ( !next || SerializedProperty.EqualContents( it, end_sp ) )
			{
				break;
			}
		}
	}
}
