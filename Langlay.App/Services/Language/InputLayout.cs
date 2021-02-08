using System;

namespace Product {
    public class InputLayout : InputLanguage {
        public string LayoutId { get => Name; }
        public string Name { get; private set; }
        public IntPtr Handle { get; private set; }

        public InputLayout(System.Windows.Forms.InputLanguage inputLanguage) : base(inputLanguage.Culture) {
            Handle = inputLanguage.Handle;
            Name = inputLanguage.LayoutName;
        }

        public override string ToString() {
            return $"{LanguageId} - {LayoutId}";
        }
    }
}