using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SKON_TabWelldingInspection
{
    // 디자이너/런타임 공용 Base Form
    public class BaseConfigForm : IDMAX_FrameWork.MaterialForm
    {
        // 변경 여부 추적
        protected bool _isDirty = false;

        // 생성자
        public BaseConfigForm()
        {
            // 디자이너가 Form을 생성할 때는 MaterialForm 내부 로직을 타면 안 됨
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return;
            }

            // 런타임에서만 초기화
            InitializeBase();
        }

        // 런타임 초기화
        private void InitializeBase()
        {
        }

        // 값 변경 감지
        protected void MarkDirty(object sender, EventArgs e)
        {
            if (!_isDirty)
                _isDirty = true;
        }

        // 저장 완료 시 호출
        protected void ResetDirty()
        {
            _isDirty = false;
        }

        // Save Confirm 공통화
        protected bool ConfirmSave()
        {
            return MessageBox.Show(
                "Would you like to save the changes?",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            ) == DialogResult.Yes;
        }

        // Close 시 Dirty 확인
        protected bool ConfirmCloseIfDirty()
        {
            if (!_isDirty)
                return true;

            return MessageBox.Show(
                "저장되지 않은 변경사항이 있습니다.\n종료하시겠습니까?",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            ) == DialogResult.Yes;
        }

        // Form 닫기 공통 처리
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // 디자이너에서는 종료 로직 타지 않음
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                base.OnFormClosing(e);
                return;
            }

            if (!ConfirmCloseIfDirty())
            {
                e.Cancel = true;
                return;
            }

            base.OnFormClosing(e);
        }
    }
}
