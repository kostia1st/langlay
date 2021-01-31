using System;

namespace Product {
    public class InputLayout : InputLanguage {
        public string Name { get; private set; }
        public int Id { get; private set; }
        public IntPtr Handle { get; private set; }

        public InputLayout(System.Windows.Forms.InputLanguage inputLanguage) : base(inputLanguage.Culture) {
            Handle = inputLanguage.Handle;
            Name = inputLanguage.LayoutName;
            Id = Name.GetHashCode();
        }

        public override string ToString() {
            return $"{LanguageName} - {Name} - {Id}";
        }
    }
}