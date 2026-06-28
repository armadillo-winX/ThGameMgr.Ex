namespace ThGameMgr.Ex.Masicalan

open Masicalan.Core
open System
open System.Collections.Generic

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

    // C# のデリゲートを (Value list -> Value) へ変換
    let private transferCsDelegate (deleg: Delegate): (Value list -> Value) =
        fun (args: Value list) ->
            let csArgs =
                args |> List.map toCsObject |> List.toArray

            // デリゲートを動的に呼び出し
            deleg.DynamicInvoke(csArgs) |> toMasicalanVal

    type FunctionEnvironmentBuilder() =
        let env = Map.empty<string, string list * Statement>

        /// <summary>
        /// Extension 関数環境に対して関数を登録します．
        /// name: 関数の名前
        /// paramName: 引数名リスト
        /// deleg: 登録する関数
        /// </summary>
        member this.Register (name: string) (paramNames: string list) (deleg: Delegate) =
            let nativeFunction = transferCsDelegate deleg
            let dummyArgs = paramNames |> List.map Expression.Var
            let statement = Statement.CallNativeF (nativeFunction, dummyArgs)

            env.Add(name, (paramNames, statement))

        /// <summary>
        /// 構築された関数環境を取得します．
        /// </summary>
        member this.Build() =
            env