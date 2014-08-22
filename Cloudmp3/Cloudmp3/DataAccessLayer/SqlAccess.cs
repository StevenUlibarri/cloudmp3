using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloudmp3.DataAccessLayer
{
	public class SqlAccess
	{
		public SqlAccess()
		{

		}

		public bool ValidateUserName(string usrName, string pass)
		{
			bool valid = false;
			using (var context = new CloudMp3SQLContext())
			{
				var query = (from u in context.Users
						  where u.U_UserName == usrName
						  select u).SingleOrDefault();
				if (query != null)
				{
					if (pass == query.U_Password)
					{
						valid = true;
					}
				}
			}
			return valid;
		}

		public int GetUserID(string usrName)
		{
			int id;
			using (var context = new CloudMp3SQLContext())
			{
				var query = (from u in context.Users
							 where u.U_UserName == usrName
							 select u).SingleOrDefault();
				id = query.U_Id;
			}
			return id;
		}

		public ObservableCollection<Song> GetSongsForUser(int userId)
		{
			ObservableCollection<Song> userSongs = new ObservableCollection<Song>();

			using (var context = new CloudMp3SQLContext())
			{
				var query = (from s in context.Songs
							 where s.S_OwnerId == userId
							 select s).ToList<Song>();
				foreach (Song x in query)
				{
					userSongs.Add(x);
				}
			}
			return userSongs;
		}

		public ObservableCollection<Playlist> GetPlaylistsForUser(int userId)
		{
			ObservableCollection<Playlist> userPlaylists = new ObservableCollection<Playlist>();
	
	        using (var context = new CloudMp3SQLContext())
	        {
		        var query = (from p in context.Playlists
						        where p.P_OwnerId == userId
						        select p).ToList<Playlist>();
		        foreach (Playlist x in query)
		        {
			        userPlaylists.Add(x);
		        }
	        }
            return userPlaylists;
        }

        
    }
}
