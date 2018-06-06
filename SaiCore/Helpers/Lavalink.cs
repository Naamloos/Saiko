using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebSocket4Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DSharpPlus;
using System.Net.Http;
using System.Linq;
using DSharpPlus.EventArgs;

namespace SaiCore.Helpers
{
	public class Lavalink
	{
		WebSocket ws;
		private string _password;
		private int _num_shards;
		private int _websocketport;
		private int _restport;
		private string _host;
		private DiscordClient _client;
		private Dictionary<ulong, string> _session_ids;
		private List<ulong> _active_guilds;

		public event AsyncEventHandler<LavalinkEvent> LavalinkEventReceived
		{
			add { this._lavalinkEventReceived.Register(value); }
			remove { this._lavalinkEventReceived.Unregister(value); }
		}
		private AsyncEvent<LavalinkEvent> _lavalinkEventReceived;

		public Lavalink(string password, int num_shards, int websocketport, int restport, ulong user_id, DiscordClient client, string host = "127.0.0.1")
		{
			this._password = password;
			this._num_shards = num_shards;
			this._client = client;
			this._websocketport = websocketport;
			this._restport = restport;
			this._host = host;

			List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>();
			headers.Add(new KeyValuePair<string, string>("Authorization", _password));
			headers.Add(new KeyValuePair<string, string>("Num-shards", _num_shards.ToString()));
			headers.Add(new KeyValuePair<string, string>("User-Id", _client.CurrentUser.Id.ToString()));

			this.ws = new WebSocket($"ws://{host}:{websocketport}", customHeaderItems: headers);

			this.ws.MessageReceived += OnMessage;
			this.ws.Error += OnError;
			this.ws.Opened += OnOpen;
			this.ws.Closed += OnClose;

			this._client.VoiceServerUpdated += _client_VoiceServerUpdated;
			this._client.VoiceStateUpdated += _client_VoiceStateUpdated;
			this._session_ids = new Dictionary<ulong, string>();
			this._active_guilds = new List<ulong>();

			this._lavalinkEventReceived = new AsyncEvent<LavalinkEvent>(EventErrorHandler, "EventReceived");
		}

		internal void EventErrorHandler(string evname, Exception ex)
		{
			Console.WriteLine($"oof oof error in {evname}\n{ex.ToString()}\n");
		}

		/// <summary>
		/// Plays a song to a guild. Make sure to connect to a voice channel first!
		/// </summary>
		/// <param name="song">Resolved song object. Use ResolveSongAsync to resolve a song!</param>
		/// <param name="guild_id">Guild to play to.</param>
		public void PlaySong(LavalinkSongResolve song, ulong guild_id)
		{
			TestConnect();
			var payload = new LavalinkPlay()
			{
				GuildId = guild_id.ToString(),
				Track = song.Track,
				StartTime = 0,
				EndTime = song.Info.Length
			};

			ws.Send(JsonConvert.SerializeObject(payload));

			if (!_active_guilds.Contains(guild_id))
				_active_guilds.Add(guild_id);
		}

		/// <summary>
		/// Stops playing in this guild.
		/// </summary>
		/// <param name="guild_id">Guild to stop playing in.</param>
		public void StopSong(ulong guild_id)
		{
			TestConnect();
			var payload = new LavalinkStop()
			{
				GuildId = guild_id.ToString()
			};

			ws.Send(JsonConvert.SerializeObject(payload));

			if (_active_guilds.Contains(guild_id))
				_active_guilds.RemoveAll(x => x == guild_id);
		}

		/// <summary>
		/// Returns whether music is being played in this guild.
		/// </summary>
		/// <param name="guild_id">Guild to check for.</param>
		/// <returns></returns>
		public bool IsPlaying(ulong guild_id) => _active_guilds.Contains(guild_id);

		/// <summary>
		/// Resolves a song link for use with Lavalink.
		/// </summary>
		/// <param name="song">Song url to resolve.</param>
		/// <returns></returns>
		public async Task<List<LavalinkSongResolve>> ResolveSongAsync(string song)
		{
			using (HttpClient http = new HttpClient())
			{
				http.DefaultRequestHeaders.Add("Authorization", _password);
				try
				{
					var resp = await http.GetStringAsync($"http://{_host}:{_restport}/loadtracks?identifier={song}");
					JArray j = JArray.Parse(resp);
					List<LavalinkSongResolve> songs = new List<LavalinkSongResolve>();
					foreach (var obj in j)
					{
						songs.Add(obj.ToObject<LavalinkSongResolve>());
					}
					return songs;
				}
				catch (Exception ex)
				{
					Console.WriteLine($"ech\n{ex.ToString()}");
					return null;
				}
			}
		}

		/// <summary>
		/// This (un)pauses a song.
		/// </summary>
		/// <param name="guild_id">Guild to (un)pause in.</param>
		/// <param name="paused">Whether you want to pause or unpause.</param>
		public void PauseSong(ulong guild_id, bool paused)
		{
			TestConnect();
			var payload = new LavalinkPause()
			{
				GuildId = guild_id.ToString(),
				Pause = paused
			};

			if (_active_guilds.Contains(guild_id))
				ws.Send(JsonConvert.SerializeObject(payload));
		}

		/// <summary>
		/// Seeks to a position in a song.
		/// </summary>
		/// <param name="guild_id">Guild to seek in.</param>
		/// <param name="positon">Position to seek to. (milliseconds)</param>
		public void SeekSong(ulong guild_id, int positon)
		{
			TestConnect();
			var payload = new LavalinkSeek()
			{
				GuildId = guild_id.ToString(),
				Position = positon
			};

			if (_active_guilds.Contains(guild_id))
				ws.Send(JsonConvert.SerializeObject(payload));
		}

		/// <summary>
		/// Sets player volume in a guild.
		/// </summary>
		/// <param name="guild_id">Guild to set volume in.</param>
		/// <param name="volume">New volume (ranges from 0 to 150).</param>
		public void SetVolume(ulong guild_id, int volume)
		{
			TestConnect();
			if (volume < 0 || volume > 150)
				throw new ArgumentOutOfRangeException("Volume may only be between 0 and 150!!");

			var payload = new LavalinkVolume()
			{
				GuildId = guild_id.ToString(),
				Volume = volume
			};

			if (_active_guilds.Contains(guild_id))
				ws.Send(JsonConvert.SerializeObject(payload));
		}

		private async Task _client_VoiceStateUpdated(VoiceStateUpdateEventArgs e)
		{
			try
			{
				if (_session_ids.Keys.Contains(e.Guild.Id))
					_session_ids.Remove(e.Guild.Id);
				_session_ids.Add(e.Guild.Id, e.SessionId);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"vsu: \n{ex.ToString()}\n");
			}
		}

		private async Task _client_VoiceServerUpdated(VoiceServerUpdateEventArgs e)
		{
			try
			{
				var payload = new LavalinkVoiceUpdate()
				{
					Event = new VoiceServerUpdate()
					{
						Endpoint = e.Endpoint,
						GuildId = e.Guild.Id.ToString(),
						Token = e.VoiceToken
					},
					GuildId = e.Guild.Id.ToString(),
					SessionId = _session_ids[e.Guild.Id]
				};

				ws.Send(JsonConvert.SerializeObject(payload));
			}
			catch (Exception ex)
			{
				Console.WriteLine($"vsu: \n{ex.ToString()}\n");
			}
		}

		private void OnMessage(object sender, MessageReceivedEventArgs e)
		{
			// WS Message!
			var obj = JObject.Parse(e.Message);
			string opcode = obj["op"].ToString();

			switch (opcode)
			{
				default:
					Console.WriteLine($"Unknown message received:\n{e.Message}\n");
					break;
				case "event":
					var ev = obj.ToObject<LavalinkEvent>();
					ev.Lavalink = this;
					HandleEvent(ev).ConfigureAwait(false).GetAwaiter().GetResult();
					break;
			}
		}

		public void TestConnect()
		{
			if (ws.State != WebSocketState.Open)
				ws.Open();
		}

		private async Task HandleEvent(LavalinkEvent e)
		{
			await _lavalinkEventReceived.InvokeAsync(e);
		}

		private void OnError(object sender, EventArgs e)
		{
			// WS Error...
		}

		private void OnOpen(object sender, EventArgs e)
		{
			// WS Open!
		}

		private void OnClose(object sender, EventArgs e)
		{
			// WS Close...
		}

		public async Task ConnectAsync()
		{
			await ws.OpenAsync();
		}
	}

	internal class VoiceServerUpdate
	{
		[JsonProperty("token")]
		public string Token;

		[JsonProperty("guild_id")]
		public string GuildId;

		[JsonProperty("endpoint")]
		public string Endpoint;
	}

	public class LavalinkSongResolve
	{
		[JsonProperty("track")]
		internal string Track;

		[JsonProperty("info")]
		public LavalinkSongInfo Info;
	}

	public class LavalinkSongInfo
	{
		[JsonProperty("identifier")]
		public string Identifier;

		[JsonProperty("isSeekable")]
		public bool IsSeekable;

		[JsonProperty("author")]
		public string Author;

		[JsonProperty("length")]
		public long Length;

		[JsonProperty("isStream")]
		public bool IsStream;

		[JsonProperty("position")]
		public long Position;

		[JsonProperty("title")]
		public string Title;

		[JsonProperty("uri")]
		public string Uri;
	}

	public class LavalinkMessage
	{
		[JsonProperty("op")]
		public string Opcode;

		[JsonIgnore]
		public Lavalink Lavalink;
	}

	public class LavalinkVoiceUpdate : LavalinkMessage
	{
		internal LavalinkVoiceUpdate()
		{
			this.Opcode = "voiceUpdate";
		}

		[JsonProperty("guildId")]
		public string GuildId;

		[JsonProperty("sessionId")]
		public string SessionId;

		[JsonProperty("event")]
		internal VoiceServerUpdate Event;
	}

	public class LavalinkPlay : LavalinkMessage
	{
		internal LavalinkPlay()
		{
			this.Opcode = "play";
		}

		[JsonProperty("guildId")]
		public string GuildId;

		[JsonProperty("track")]
		public string Track;

		[JsonProperty("startTime")]
		public long StartTime;

		[JsonProperty("endTime")]
		public long EndTime;
	}

	public class LavalinkStop : LavalinkMessage
	{
		internal LavalinkStop()
		{
			this.Opcode = "stop";
		}

		[JsonProperty("guildId")]
		public string GuildId;
	}

	public class LavalinkPause : LavalinkMessage
	{
		internal LavalinkPause()
		{
			this.Opcode = "pause";
		}

		[JsonProperty("guildId")]
		public string GuildId;

		[JsonProperty("pause")]
		public bool Pause;
	}

	public class LavalinkSeek : LavalinkMessage
	{
		internal LavalinkSeek()
		{
			this.Opcode = "seek";
		}

		[JsonProperty("guildId")]
		public string GuildId;

		[JsonProperty("position")]
		public long Position;
	}

	public class LavalinkVolume : LavalinkMessage
	{
		internal LavalinkVolume()
		{
			this.Opcode = "volume";
		}

		[JsonProperty("guildId")]
		public string GuildId;

		[JsonProperty("volume")]
		public int Volume;
	}

	public class LavalinkDestroy : LavalinkMessage
	{
		internal LavalinkDestroy()
		{
			this.Opcode = "destroy";
		}

		[JsonProperty("guildId")]
		public string GuildId;
	}

	public class LavalinkPlayerUpdate : LavalinkMessage
	{
		internal LavalinkPlayerUpdate()
		{
			this.Opcode = "playerUpdate";
		}

		[JsonProperty("guildId")]
		public string GuildId;
	}

	public class LavalinkState
	{
		internal LavalinkState() { }

		[JsonProperty("time")]
		public long Time;

		[JsonProperty("position")]
		public long Position;
	}

	public class LavalinkEvent : AsyncEventArgs
	{
		[JsonIgnore]
		public Lavalink Lavalink;

		[JsonProperty("op")]
		public string Opcode { get { return "event"; } }

		[JsonProperty("reason")]
		public string Reason;

		[JsonProperty("type")]
		public string Type;

		[JsonProperty("track")]
		public string Track;

		[JsonProperty("guildId")]
		public string GuildId;
	}
}
