using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.Frame.Item.Text;

internal class TextComponentCollection
{
    // Internal fields.
    internal Vector2 MaxDimensions
    {
        get => _maxDimensions; 
        set => _maxDimensions = value;
    }

    internal int MaxCharacterCount { get; set; } = int.MaxValue;



    internal TextComponent[] UserComponents
    {
        get => _userComponents;
        set
        {
            _userComponents = value ?? throw new ArgumentNullException(nameof(value));
            Format();
        }
    }
    
    internal FormattedTextComponent[] FormattedComponents { get; private set; }


    // Private fields.
    private TextComponent[] _userComponents;
    private Vector2 _maxDimensions = new(float.PositiveInfinity, float.PositiveInfinity);


    // Constructors.
    internal TextComponentCollection() { }


    // Internal methods.
    internal void Format()
    {
        List<FormattedTextComponent> Components = new(UserComponents.Length);

        for (int i = 0; i < UserComponents.Length; i++)
        {

        }
    }


    // Private methods.
    private void Load
}