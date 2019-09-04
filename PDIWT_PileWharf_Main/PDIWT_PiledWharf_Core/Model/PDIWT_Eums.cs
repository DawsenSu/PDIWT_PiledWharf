using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDIWT_PiledWharf_Core.Model
{
    /// <summary>
    /// Enum type for bearing calculation type
    /// </summary>
    public enum BearingCapacityPileTypes
    {
        [Description("BCPileType_DrivenPileWithSealedEnd")]
        DrivenPileWithSealedEnd,
        [Description("BCPileType_TubePileOrSteelPile")]
        TubePileOrSteelPile,
        [Description("BCPileType_CastInSituPile")]
        CastInSituPile,
        [Description("BCPileType_CastInSituAfterGrountingPile")]
        CastInSituAfterGrountingPile
    }

    /// <summary>
    /// Enum type of project phase for outputting calculation note
    /// </summary>
    public enum PDIWT_ProjectPhase
    {
        [Description("PP_PreliminaryFeasibilityStudy")]
        Preliminary_Feasibility_Study,
        [Description("PP_FeasibilityStudy")]
        Feasibility_Study,
        [Description("PP_PreliminaryDesign")]
        PreliminaryDesign,
        [Description("PP_ConstructionDesign")]
        Construction_Design
    }

    /// <summary>
    /// Enum type to show the pile tip type
    /// </summary>
    public enum PileTipType
    {
        [Description("PileTip_TotalSeal")]
        TotalSeal,
        [Description("PileTip_HalfSeal")]
        HalfSeal,
        [Description("PileTip_SingleBorad")]
        SingleBorad,
        [Description("PileTip_DoubleBoard")]
        DoubleBoard,
        [Description("PileTip_QuadBoard")]
        QuadBoard
    }

    /// <summary>
    /// Enum type to show the result status of pile soil layercollection intersection information
    /// </summary>
    public enum PileSoilLayersInsectionStatus
    {
        NoSoilLayer,
        NotAllSoilLayersContainMeshElement,
        NoIntersection,
        Success
    }

    public enum GetPileBearingCapacityCurveInfoStatus
    {
        InvalidObjectStruct,
        NoIntersection,
        Success
    }

    /// <summary>
    /// Enum type to show the result status of pile length calculation based on pile axis ray and soil layer meshes.
    /// </summary>
    public enum CalculatePileLengthStatues
    {
        Success = 0,
        TargetBearingCapacityIsTooLarge,
        NoLayerInfos
    }
}
