using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

// From: https://amalgamatelabs.com/Blog/4/data_persistence
public static class SaveLoad
{
	[DllImport( "__Internal" )]
	private static extern void SyncFiles();

	[DllImport( "__Internal" )]
	private static extern void WindowAlert( string message );

	public static void Save()
	{
		// Convert from items to list of ids (assetpaths)
		Player.Instance.Data.Items = new string[Player.Instance.Items.Count];
		int index = 0;
		foreach ( var item in Player.Instance.Items )
		{
			Player.Instance.Data.Items[index] = item.name;
			index++;
		}

		// Convert equipped dictionary to two arrays
		int count = Player.Instance.EquippedItems.Count;
		Player.Instance.Data.EquippedItemsKey = new string[count];
		Player.Instance.Data.EquippedItemsValue = new int[count];
		index = 0;
		foreach ( var item in Player.Instance.EquippedItems )
		{
			Player.Instance.Data.EquippedItemsKey[index] = item.Key;
			Player.Instance.Data.EquippedItemsValue[index] = item.Value;
			index++;
		}
		Player.Instance.Data.BuildVersion = Application.buildGUID;

		// Save
		string dataPath = string.Format("{0}/save.dat", Application.persistentDataPath);
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		FileStream fileStream;

		try
		{
			if ( File.Exists( dataPath ) )
			{
				File.WriteAllText( dataPath, string.Empty );
				fileStream = File.Open( dataPath, FileMode.Open );
			}
			else
			{
				fileStream = File.Create( dataPath );
			}

			binaryFormatter.Serialize( fileStream, Player.Instance.Data );
			fileStream.Close();

			OnSave();
		}
		catch (Exception e)
		{
			PlatformSafeMessage("Failed to Save: " + e.Message);
		}
	}

	public static void Load()
	{
		SaveInfo data = new SaveInfo();
		string dataPath = string.Format("{0}/save.dat", Application.persistentDataPath);

		try
		{
			if ( File.Exists( dataPath ) )
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				FileStream fileStream = File.Open(dataPath, FileMode.Open);

				data = (SaveInfo) binaryFormatter.Deserialize( fileStream );
				fileStream.Close();
			}
			else
			{
				data.Gold = 0;
				data.Items = null;
				data.EquippedItemsKey = null;
				data.EquippedItemsValue = null;
			}
		}
		catch ( Exception e )
		{
			PlatformSafeMessage( "Failed to Load: " + e.Message );
		}

		if ( data.BuildVersion != Application.buildGUID ) return;

		// Apply to player data
		// Account for bad save/load
		Player.Instance.Data = data;

		// Load items from string ids (assetpath)
		Player.Instance.Items = new List<BaseItem>();
		if ( data.Items != null )
		{
			foreach ( var item in data.Items )
			{
				Player.Instance.Items.Add( Resources.Load<BaseItem>( "Items/" + item ) );
			}
		}

		// Load equipped from arrays into dictionary again
		Player.Instance.EquippedItems = new Dictionary<string, int>();
		if ( data.EquippedItemsKey != null )
		{
			for ( int i = 0; i < data.EquippedItemsKey.Length; i++ )
			{
				Player.Instance.Equip( data.EquippedItemsKey[i], Player.Instance.Items[data.EquippedItemsValue[i]] );
			}
		}
	}

	private static void OnSave()
	{
		if ( Application.platform == RuntimePlatform.WebGLPlayer )
		{
			SyncFiles();
		}
	}

	private static void PlatformSafeMessage( string message )
	{
		if ( Application.platform == RuntimePlatform.WebGLPlayer )
		{
			WindowAlert( message );
		}
		else
		{
			Debug.Log( message );
		}
	}
}
