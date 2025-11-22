using System;

namespace PDFIndexer.SetupWizard
{
    internal abstract class Phase
    {
        protected bool Initialized;

        public abstract void NextButton_Click(object sender, EventArgs e);

        public abstract void PrevButton_Click(object sender, EventArgs e);

        public abstract void Setup();

        protected abstract void Initialize();
    }
}
