using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DSharpPlus.EventArgs;
using DSharpPlus;
using System.Net.Http;
using System.Linq;

namespace SaiCore.Helpers
{
	public class Lavalink
	{
		WebSocket ws;
		private string _password;
		private int _num_shards;
		private ulong _user_id;
		private DiscordClient _client;
		private Dictionary<ulong, string> _session_ids;

		public Lavalink(string password, int num_shards, ulong user_id, DiscordClient client)
		{
			this._password = password;
			this._num_shards = num_shards;
			this._user_id = user_id;
			this._client = client;

			this.ws = new WebSocket("ws://127.0.0.1:6942")
			{
				CustomHeaders = new Dictionary<string, string>()
				{
					{ "Authorization", password },
					{ "Num-shards", num_shards.ToString() },
					{ "User-Id", user_id.ToString() }
				}
			};

			this.ws.OnMessage += OnMessage;
			this.ws.OnError += OnError;
			this.ws.OnOpen += OnOpen;
			this.ws.OnClose += OnClose;

			this._client.VoiceServerUpdated += _client_VoiceServerUpdated;
			this._client.VoiceStateUpdated += _client_VoiceStateUpdated;
			this._session_ids = new Dictionary<ulong, string>();
		}

		public async Task<LavalinkSongInfo> PlaySong(ulong guild_id, string song)
		{
			var resolve = await ResolveSong(song);

			var payload = new LavalinkPlay()
			{
				GuildId = guild_id.ToString(),
				Track = resolve.Track,
				StartTime = 0,
				EndTime = resolve.Info.Length
			};

			ws.Send(JsonConvert.SerializeObject(payload));
			return resolve.Info;
		}

		public async Task StopSong(ulong guild_id)
		{
			var payload = new LavalinkStop()
			{
				GuildId = guild_id.ToString()
			};

			ws.Send(JsonConvert.SerializeObject(payload));
		}

		private async Task _client_VoiceStateUpdated(VoiceStateUpdateEventArgs e)
		{
			try
			{
				if (_session_ids.Keys.Contains(e.Guild.Id))
					_session_ids.Remove(e.Guild.Id);
				_session_ids.Add(e.Guild.Id, e.SessionId);
			}catch(Exception ex)
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

		private void OnMessage(object sender, MessageEventArgs e)
		{
			// WS Message!
			Console.WriteLine($"{e.Data}\n");
		}

		private void OnError(object sender, ErrorEventArgs e)
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

		private async Task<LavalinkSongResolve> ResolveSong(string song)
		{
			using (HttpClient http = new HttpClient())
			{
				http.DefaultRequestHeaders.Add("Authorization", _password);
				try
				{
					var resp = await http.GetStringAsync($"http://127.0.0.1:2333/loadtracks?identifier={song}");
					JArray j = JArray.Parse(resp);
					return j[0].ToObject<LavalinkSongResolve>();
				}
				catch(Exception ex)
				{
					Console.WriteLine($"ech\n{ex.ToString()}");
					return null;
				}
			}
		}

		public void Connect()
		{
			ws.Connect();
		}
	}

	class VoiceServerUpdate
	{
		[JsonProperty("token")]
		public string Token;

		[JsonProperty("guild_id")]
		public string GuildId;

		[JsonProperty("endpoint")]
		public string Endpoint;
	}

	class LavalinkSongResolve
	{
		[JsonProperty("track")]
		public string Track;

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

	class LavalinkMessage
	{
		[JsonProperty("op")]
		public string Opcode;
	}

	class LavalinkVoiceUpdate : LavalinkMessage
	{
		public LavalinkVoiceUpdate()
		{
			this.Opcode = "voiceUpdate";
		}

		[JsonProperty("guildId")]
		public string GuildId;

		[JsonProperty("sessionId")]
		public string SessionId;

		[JsonProperty("event")]
		public VoiceServerUpdate Event;
	}

	class LavalinkPlay : LavalinkMessage
	{
		public LavalinkPlay()
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

	class LavalinkStop : LavalinkMessage
	{
		public LavalinkStop()
		{
			this.Opcode = "stop";
		}

		[JsonProperty("guildId")]
		public string GuildId;
	}

	class LavalinkPause : LavalinkMessage
	{
		public LavalinkPause()
		{
			this.Opcode = "pause";
		}

		[JsonProperty("guildId")]
		public string GuildId;

		[JsonProperty("pause")]
		public bool Pause;
	}

	class LavalinkSeek : LavalinkMessage
	{
		public LavalinkSeek()
		{
			this.Opcode = "seek";
		}

		[JsonProperty("guildId")]
		public string GuildId;

		[JsonProperty("position")]
		public long Position;
	}

	class LavalinkDestroy : LavalinkMessage
	{
		public LavalinkDestroy()
		{
			this.Opcode = "destroy";
		}

		[JsonProperty("guildId")]
		public string GuildId;
	}

	class LavalinkPlayerUpdate : LavalinkMessage
	{
		public LavalinkPlayerUpdate()
		{
			this.Opcode = "playerUpdate";
		}

		[JsonProperty("guildId")]
		public string GuildId;
	}

	class LavalinkState
	{
		[JsonProperty("time")]
		public long Time;

		[JsonProperty("position")]
		public long Position;
	}

	class LavalinkEvent : LavalinkMessage
	{
		public LavalinkEvent()
		{
			this.Opcode = "event";
		}
	}
}
