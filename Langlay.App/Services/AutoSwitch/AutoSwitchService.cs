using System;
using System.Linq;
using Product.Common;

#if TRACE
using System.Diagnostics;
#endif

namespace Product {
	public class AutoSwitchService : IAutoSwitchService, ILifecycled {
		private IEventService EventService { get; set; }
		private LastInputTimer Timer { get; set; }

		#region Start/Stop
		public bool IsStarted { get; private set; }

		public void Start() {
			if (!IsStarted) {
				IsStarted = true;
				Timer = new LastInputTimer {
					OnTimer = HandleTimer
				};

				EventService = ServiceRegistry.Instance.Get<IEventService>();
				EventService.KeyboardInput += Timer.SignalInput;
				EventService.MouseInput += Timer.SignalInput;

				Timer.Start();
			}
		}

		public void Stop() {
			if (IsStarted) {
				IsStarted = false;

				EventService.KeyboardInput -= Timer.SignalInput;
				EventService.MouseInput -= Timer.SignalInput;

				Timer.Stop();
				Timer = null;
				LastFocusedWindowHandle = null;
			}
		}
		#endregion Start/Stop

		private IntPtr? LastFocusedWindowHandle { get; set; }

		private void HandleTimer() {
			var configService = ServiceRegistry.Instance.Get<IConfigService>();
			if (configService.AppAttachmentArray.Count > 0) {
				var languageService = ServiceRegistry.Instance.Get<ILanguageService>();
				var currentFocusedWindowHandle = Win32.GetForegroundWindow();
				if (currentFocusedWindowHandle != IntPtr.Zero) {
					if (
						LastFocusedWindowHandle != null
						&& LastFocusedWindowHandle != currentFocusedWindowHandle
					) {
						var text = Win32.GetWindowText(currentFocusedWindowHandle);
#if TRACE
						Trace.WriteLine("Active window title: " + text);
#endif
						var attachment = configService.AppAttachmentArray.FirstOrDefault(x => text.Contains(x.AppMask));
						if (attachment != null) {
							var inputLayouts = languageService.GetInputLayouts();
							var layout = inputLayouts.FirstOrDefault(x => x.Id.ToString() == attachment.LanguageOrLayoutId);
							if (layout != null) {
#if TRACE
								Trace.WriteLine($"Attempting to restore layout {layout.LanguageName} - {layout.Name}");
#endif
								languageService.SetCurrentLayout(layout);
							} else {
#if TRACE
								Trace.WriteLine($"Attempting to restore language {attachment.LanguageOrLayoutId}");
#endif
								languageService.SetCurrentLanguage(attachment.LanguageOrLayoutId, true);
							}
						}
					}
					LastFocusedWindowHandle = currentFocusedWindowHandle;
				}
			}
		}
	}
}
