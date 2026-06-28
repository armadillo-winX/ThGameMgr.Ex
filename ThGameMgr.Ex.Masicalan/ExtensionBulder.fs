namespace ThGameMgr.Ex.Masicalan

open Masicalan.Core
open System

module ExtensionBulder =
    let rec private toMasicalanVal (o: obj) : Value =
        match o with
        | :? int as i -> Value.IntVal i
        | :? float as f -> Value.FloatVal f
        | :? string as s -> Value.StringVal s
        | :? bool as b -> Value.BoolVal b
        | ob when ob.GetType() = typeof<Void> -> Value.VoidVal
        | :? list<obj> as l -> Value.ArrayVal (List.map toMasicalanVal l)
        |_ -> failwithf $"Unsupported type: {o.GetType().Name}"

    let rec private toCsObject (v: Value): obj =
        match v with
        | Value.IntVal i -> i
        | Value.FloatVal f -> f
        | Value.StringVal s -> s
        | Value.BoolVal b -> b
        | Value.VoidVal -> null
        | Value.ArrayVal a -> List.map toCsObject a
