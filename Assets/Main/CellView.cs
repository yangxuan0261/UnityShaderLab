using System;
using System.Collections;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using UnityEngine;
using UnityEngine.UI;

namespace Demo {

    public class CellView : EnhancedScrollerCellView {

        public Text title;
        public Action<CellView, Data> onClickFn;

        private Data mData;

        public void SetData(Data data, int dataIndex) {
            mData = data;

            title.text = string.Format(" {0:D2}. {1}", dataIndex + 1, data.title);
        }

        public void OnClick() {
            onClickFn(this, mData);
        }
    }
}