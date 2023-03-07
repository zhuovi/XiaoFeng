using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.IO;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-03-02 08:51:03                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Http
{
	/// <summary>
	/// HttpSocket 请求操作类
	/// </summary>
	public class HttpSocket : Disposable
	{
		#region 构造器
		/// <summary>
		/// 设置请求对象
		/// </summary>
		/// <param name="request">请求对象</param>
		public HttpSocket(HttpRequest request)
		{
			Request = request;
		}

		#endregion

		#region 属性
		/// <summary>
		/// 请求对象
		/// </summary>
		public HttpRequest Request { get; set; }
		/// <summary>
		/// 响应对象
		/// </summary>
		public HttpResponse Response { get; set; }
		/// <summary>
		/// 请求Uri
		/// </summary>
		public Uri RequestUri { get; set; }
		/// <summary>
		/// 网络流
		/// </summary>
		public Stream NetStream { get; set; }
		/// <summary>
		/// 转向次数
		/// </summary>
		public int RedirectCount { get; set; } = 0;
		#endregion

		#region 方法

		#region 创建请求头
		/// <summary>
		/// 创建请求头
		/// </summary>
		/// <param name="uri">请求网址</param>
		/// <returns></returns>
		public StringBuilder CreateRequestHeader(Uri uri)
		{
			var header = new StringBuilder();
			if (this.Request.WebProxy != null)
			{
				header.Append($"{this.Request.Method.Method.ToUpper()} {uri.Scheme}://{uri.Host}:{uri.Port}{uri.PathAndQuery} HTTP/{this.Request.ProtocolVersion}\r\n");
				header.Append($"Proxy-Connection:keep-alive\r\n");
				var credentials = this.Request.WebProxy.Credentials.GetCredential(uri, "Basic");
				header.Append($"Proxy-Authorization:Basic {(credentials.UserName + ":" + credentials.Password).ToBase64String()}\r\n");
			}
			else
			{
				header.Append($"{this.Request.Method.Method.ToUpper()} {uri.PathAndQuery} HTTP/{this.Request.ProtocolVersion}\r\n");
			}
			if (uri.Scheme.ToUpper() == "HTTPS")
			{
				header.Append($":authority:{uri.Host}\r\n");
				header.Append($":method:{this.Request.Method.Method.ToUpper()}\r\n");
				header.Append($":path:{uri.AbsolutePath}\r\n");
				header.Append($":scheme:{uri.Scheme}\r\n");
			}
			if (this.Request.Credentials != null)
			{
				var credentials = this.Request.Credentials.GetCredential(RequestUri, "Basic");
				header.Append($"Authorization:Basic {(credentials.UserName + ":" + credentials.Password).ToBase64String()}\r\n");
			}
			header.Append($"Accept:{this.Request.Accept}\r\n");
			header.Append($"Accept-Encoding:{this.Request.AcceptEncoding.Multivariate("gzip, deflate")}\r\n");
			header.Append($"Accept-Language:{this.Request.AcceptLanguage.Multivariate("zh-CN,zh;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6")}\r\n");
			header.Append($"Cache-Control:{(this.Request.CacheControl == null ? "max-age=0" : this.Request.CacheControl.ToString())}\r\n");
			header.Append($"Connection:{(this.Request.KeepAlive ? "keep-alive" : "close")}\r\n");
			if (this.Request.CookieContainer != null && this.Request.CookieContainer.Count > 0)
			{
				var Cookie = new List<string>();
				var cookies = this.Request.CookieContainer.GetCookies(uri);
				for (var i = 0; i < cookies.Count; i++)
				{
					var cookie = cookies[i];
					Cookie.Add($"{cookie.Name}={cookie.Value}");
				}
				header.Append($"{Cookie}:{Cookie.Join("&")}\r\n");
			}
			header.Append($"Host:{uri.Host}:{uri.Port}\r\n");
			header.Append($"Origin:{uri.Scheme}://{uri.Host}:{uri.Port}\r\n");
			header.Append($"Upgrade-Insecure-Requests:1\r\n");
			header.Append($"User-Agent:{this.Request.UserAgent}\r\n");
			if (this.Request.Referer.IsNotNullOrEmpty())
				header.Append($"Referer:{this.Request.Referer}\r\n");

			if (this.Request.ContentType.IsNotNullOrWhiteSpace())
			{
				header.Append($"Content-type:{this.Request.ContentType}\r\n");
			}
			else
			{
				if (this.Request.Method.Method.ToUpper() == "POST")
				{
					header.Append($"Content-Type:application/x-www-form-urlencoded");
				}
			}
			if (this.Request.Encoding != null)
				header.Append($"Accept-Charset:{this.Request.Encoding.EncodingName}\r\n");

			if (this.Request.Expect100Continue)
				header.Append($"Expect:100-continue\r\n");
			if (this.Request.ContentLength > 0)
				header.Append($"Content-Length:{this.Request.ContentLength}\r\n");

			if (this.Request.IfModifiedSince != null)
				header.Append($"If-Modified-Since:{this.Request.IfModifiedSince:r}\r\n");
			if (this.Request.Headers != null && this.Request.Headers.Count > 0)
			{
				this.Request.Headers.Each(h =>
				{
					header.Append($"{h.Key}:{h.Value}\r\n");
				});
			}

			header.Append("\r\n");
			return header;
		}
		#endregion

		#region 创建NetworkStream
		/// <summary>
		/// 创建NetworkStream
		/// </summary>
		/// <returns></returns>
		public NetworkStream CreateNetStream()
		{
			var uri = new Uri(this.Request.Address);
			if (this.Request.WebProxy != null)
				uri = this.Request.WebProxy.Address;
			var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
			{
				NoDelay = true,
				ReceiveTimeout = this.Request.Timeout,
				SendTimeout = this.Request.Timeout
			};
			try
			{
				socket.Connect(Dns.GetHostAddresses(uri.Host), uri.Port);
				return new NetworkStream(socket, true)
				{
					ReadTimeout = socket.ReceiveTimeout,
					WriteTimeout = socket.SendTimeout
				};
			}
			catch (SocketException ex)
			{
				throw ex;
			}
		}
		#endregion

		#region 发送请求
		/// <summary>
		/// 发送请求
		/// </summary>
		/// <returns></returns>
		public async Task<HttpResponse> SendRequestAsync()
		{
			this.Response = new HttpResponse();
			var url = this.Request.Address;
			if (this.Request.Method.Method == "Get" && this.Request.Data != null && this.Request.Data.Count > 0)
			{
				this.Request.Data.Each(k =>
				{
					url += (url.Contains("?") ? "&" : "?") + k.Key + "=" + k.Value.UrlEncode();
				});
			}
			this.RequestUri = new Uri(url);

			var NetStream = this.CreateNetStream();
			this.NetStream = this.RequestUri.Scheme.ToUpper() == "HTTP" ? NetStream as Stream : this.GetSslStream(NetStream);

			this.Response.Request = this.Request;
			this.Response.SetBeginTime();

			return await this.SendRequestAsync(this.RequestUri);
		}
		#endregion

		#region 发送请求
		/// <summary>
		/// 发送请求
		/// </summary>
		/// <param name="requestUri">请求地址</param>
		/// <returns></returns>
		private async Task<HttpResponse> SendRequestAsync(Uri requestUri)
		{
			byte[] RequestBody = Array.Empty<byte>();
			if (this.Request.Method == "POST")
			{
				RequestBody = this.Request.GetReuqestBody();
			}
			var RequestHeader = this.CreateRequestHeader(requestUri);
			var bytes = RequestHeader.ToString().GetBytes(this.Request.Encoding);
			//发送头
			await this.NetStream.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
			if (RequestBody.Length > 0)
			{
				//发送body
				await this.NetStream.WriteAsync(RequestBody, 0, RequestBody.Length).ConfigureAwait(false);
			}
			await this.NetStream.FlushAsync().ConfigureAwait(false);

			byte[] buffer;
			var buffers = new MemoryStream();

			if (this.NetStream is NetworkStream ns)
			{
				do
				{
					buffer = new byte[1024];
					var length = await ns.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
					if (length == 0) break;
					await buffers.WriteAsync(buffer, 0, length).ConfigureAwait(false);
				} while (ns.DataAvailable);
			}
			else
			{
				var ssl = this.NetStream as SslStream;
				do
				{
					buffer = new byte[1024];
					var length = await ssl.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
					if (length == 0) break;
					await buffers.WriteAsync(buffer, 0, length).ConfigureAwait(false);

					var array = buffers.ToArray();
					if (array.Length >= 7 && array.Skip(array.Length - 7).Take(7).ToArray().GetString(this.Request.Encoding) == "\r\n0\r\n\r\n") break;
				} while (ssl.CanRead);
			}
			/*while (true)
			{
				buffer = new byte[1024];
				var length = await this.NetStream.ReadAsync(buffer, 0, buffer.Length);
				if (length == 0) break;
				await buffers.WriteAsync(buffer, 0, length);
				//if (length < buffer.Length) break;
			}*/
			
			var Headers = this.GetResponseHeaders(buffers);
			if (Headers.TryGetValue("Location", out var location))
			{
				if (this.Request.AllowAutoRedirect)
				{
					if (!location.IsSite())
						location = $"{requestUri.Scheme}://{requestUri.Host}:{requestUri.Port}{location}";
					var uri = new Uri(location);
					this.RedirectCount++;
					if (RedirectCount <= this.Request.MaximumAutomaticRedirections)
					{
						if (this.Response.ResponseUri == null) this.Response.ResponseUris = new List<Uri>();
						this.Response.ResponseUri = uri;
						this.Response.ResponseUris.Add(uri);
						return await this.SendRequestAsync(uri).ConfigureAwait(false);
					}
					else
					{
						throw new Exception("转向地址次数超过了设置最大数量.");
					}
				}
			}
			if (this.Response.IsChunked)
			{
				var End = false;
				var BlockLine = new MemoryStream();
				var Body = false;
				var BodyStream = new MemoryStream();
				while (buffers.CanRead && buffers.Position < buffers.Length)
				{
					var c = buffers.ReadByte();
					if (End)
					{
						if (c == 10)
						{
							if (Body)
							{
								var _bytes = BlockLine.ToArray();
								BodyStream.Write(_bytes, 0, _bytes.Length);
								BlockLine.SetLength(0);
								Body = false;
							}
							else
							{
								if (BlockLine.Length == 0) continue;
								var length = Convert.ToInt32(BlockLine.ToArray().GetString(this.Request.Encoding), 16);
								if (length == 0) break;
								var BodyBytes = new byte[length];
								await buffers.ReadAsync(BodyBytes, 0, BodyBytes.Length);
								await BodyStream.WriteAsync(BodyBytes, 0, length);
								BlockLine.SetLength(0);
								Body = false;
							}
							End = false;
						}
						else
						{
							BlockLine.WriteByte((byte)c);
							End = false;
						}
					}
					else if (c == 13)
						End = true;
					else
						BlockLine.WriteByte((byte)c);
				}
				Response.Data = BodyStream.ToArray();

				BodyStream.Close();
				BodyStream.Dispose();
				BlockLine.Close();
				BlockLine.Dispose();
			}

			buffers.Close();
			buffers.Dispose();

			this.Response.SetEndTime();

			this.Response.HttpCore = HttpCore.HttpSocket;
			Response.Headers = Headers;
			await Response.InitSocketAsync().ConfigureAwait(false);
			this.Close();
			return this.Response;
		}
		#endregion

		#region 获取响应头
		/// <summary>
		/// 获取响应头
		/// </summary>
		/// <param name="resonseStream">响应流</param>
		/// <returns></returns>
		private IDictionary<string, string> GetResponseHeaders(MemoryStream resonseStream)
		{
			var Headers = new Dictionary<string, string>();
			resonseStream.Seek(0, SeekOrigin.Begin);

			var End = false;
			var BlockLine = new MemoryStream();
			var uri = new Uri(this.Request.Address);
			while (resonseStream.CanRead && resonseStream.Position < resonseStream.Length)
			{
				var c = resonseStream.ReadByte();
				if (End)
				{
					if (c == 10)
					{
						if (BlockLine.Length == 0)
						{
							//进入body
							if (Headers.TryGetValue("Transfer-Encoding", out var transferEncoding))
							{
								//处理chunked
								this.Response.IsChunked = true;
								break;
							}
							else
							{
								var ContentLength = 0L;
								if (Headers.TryGetValue("Content-Length", out var contentLength))
								{
									ContentLength = contentLength.ToCast<long>();
								}
								else
								{
									ContentLength = resonseStream.Length - resonseStream.Position;
								}
								var BodyBytes = new byte[ContentLength];
								resonseStream.Read(BodyBytes, 0, (int)ContentLength);
								Response.Data = BodyBytes;
							}
						}
						else
						{
							var _line = BlockLine.ToArray().GetString(this.Request.Encoding);
							if (_line.StartsWith($"HTTP/{this.Request.ProtocolVersion}"))
							{
								var httpStatus = _line.GetMatchs($@"^{uri.Scheme.ToUpper()}/{this.Request.ProtocolVersion}\s+(?<a>\d+)\s+(?<b>.*?)$");
								if (httpStatus.TryGetValue("a", out var a))
								{
									Response.StatusCode = a.ToCast<int>().ToCast<HttpStatusCode>();
								}
								if (httpStatus.TryGetValue("b", out var b))
								{
									Response.StatusDescription = b;
								}
							}
							else
							{
								var kvs = _line.GetMatchs(@"^(?<k>[^:]+):\s+(?<v>[\s\S]*)$");
								if (kvs != null && kvs.Count > 0 && kvs.TryGetValue("k", out var k) && kvs.TryGetValue("v", out var v) && !Headers.ContainsKey(k))
								{
									Headers.Add(k, v);
								}
							}
							BlockLine.SetLength(0);
						}
						End = false;
						continue;
					}
					else
					{
						BlockLine.WriteByte((byte)c);
						End = false;
					}
				}
				else if (c == 13)
					End = true;
				else
					BlockLine.WriteByte((byte)c);
			}
			return Headers;
		}
		#endregion

		#region 关闭
		/// <summary>
		/// 关闭
		/// </summary>
		public void Close()
		{
			if (this.NetStream != null)
			{
				this.NetStream.Close();
				this.NetStream.Dispose();
			}
		}
		#endregion

		#region 获取ssl流
		/// <summary>
		/// 获取ssl流
		/// </summary>
		/// <param name="stream">网络流</param>
		/// <returns></returns>
		/// <exception cref="Exception">异常</exception>
		private SslStream GetSslStream(NetworkStream stream)
		{
			var ssl = new SslStream(stream, true, new RemoteCertificateValidationCallback((o, certificate, chain, errors) =>
			{
				return errors == SslPolicyErrors.None;
			}));
			if (this.Request.CertPath.IsNotNullOrEmpty())
			{
				var cert = this.Request.CertPath.GetBasePath();
				if (File.Exists(cert))
				{
					var x509 = this.Request.CertPassWord.IsNullOrEmpty() ? new X509Certificate2(cert) : new X509Certificate2(cert, this.Request.CertPassWord);
					if (this.Request.ClentCertificates == null) this.Request.ClentCertificates = new X509Certificate2Collection(x509);
					else
						this.Request.ClentCertificates.Add(x509);
				}
			}
			var uri = new Uri(this.Request.Address);
			ssl.AuthenticateAsClient(uri.Host, this.Request.ClentCertificates, System.Security.Authentication.SslProtocols.Tls12, false);
			if (ssl.IsAuthenticated)
			{
				return ssl;
			}
			else
			{
				throw new Exception("认证失败.");
			}
		}
		#endregion

		#endregion
	}
}