using System.Collections.Generic;
using System.Windows.Forms;
using Product.Common;

namespace Product {

    public class KeyStroke {

        public IList<Keys> Keys {
            get {
                var result = KeysPressedBefore;
                if (!result.Contains(KeyTriggeredEvent))
                    result = Utils.Combine(new[] { KeyTriggeredEvent }, result);
                return result;
            }
        }

        public Keys KeyTriggeredEvent { get; set; }
        public IList<Keys> KeysPressedBefore { get; set; }

        public KeyStroke(Keys keyTriggered, IList<Keys> keysPressedBefore) {
            KeyTriggeredEvent = keyTriggered;
            KeysPressedBefore = keysPressedBefore;
        }
    }
}