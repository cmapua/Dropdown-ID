using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DropdownID
{
    public class BaseIdentifiablePropertyDrawer<TAttribute, TIdentifiable> : OdinAttributeDrawer<TAttribute, int>
        where TAttribute : BaseIdentifiableAttribute
        where TIdentifiable : class, IIdentifiable
    {
        private List<TIdentifiable> _identifiables;

        protected override void DrawPropertyLayout(GUIContent label)
        {
            var rect = EditorGUILayout.GetControlRect();
            if (label != null) rect = EditorGUI.PrefixLabel(rect, label);

            if (_identifiables == null) _identifiables = FindIdentifiables();

            var serializedValueLabel = GetLabelForId(ValueEntry.SmartValue, _identifiables);

            var results = OdinSelector<ValueDropdownItem<int>>.DrawSelectorDropdown(
                rect,
                new GUIContent(string.IsNullOrEmpty(serializedValueLabel) ? "<None>" : serializedValueLabel),
                DoSelector
            );

            if (results != null) ValueEntry.SmartValue = results.FirstOrDefault().Value;
        }

        private List<TIdentifiable> FindIdentifiables()
        {
            var filter = $"t:{typeof(TIdentifiable).FullName}";
            //Debug.Log($"Filter: {filter}");
            var guids = AssetDatabase.FindAssets(filter);
            var configs = new List<TIdentifiable>();

            for (var i = 0; i < guids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var c = AssetDatabase.LoadAssetAtPath<Object>(path) as TIdentifiable;
                configs.Add(c);
            }

            return configs;
        }

        private string GetLabelForId(int id, List<TIdentifiable> identifiables)
        {
            foreach (var identifiable in identifiables)
            {
                if (identifiable.ID == id) return identifiable.DropdownOptionLabel;
            }

            return null;
        }

        // source: https://discord.com/channels/355444042009673728/355817720182341632/716002447893725255
        private OdinSelector<ValueDropdownItem<int>> DoSelector(Rect buttonRect)
        {
            var dropdown = new List<ValueDropdownItem<int>>
            {
                new ValueDropdownItem<int>("(None)", 0)
            };

            dropdown.AddRange(_identifiables.Select(a => new ValueDropdownItem<int>(a.DropdownOptionLabel, a.ID)));

            var selector = new GenericSelector<ValueDropdownItem<int>>(dropdown);
            selector.EnableSingleClickToSelect();
            selector.ShowInPopup(buttonRect);

            return selector;
        }
    }
}