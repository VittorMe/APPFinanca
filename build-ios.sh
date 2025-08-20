#!/bin/bash

echo "🚀 Iniciando build para iOS..."

# Limpar builds anteriores
echo "🧹 Limpando builds anteriores..."
dotnet clean

# Restaurar pacotes
echo "📦 Restaurando pacotes..."
dotnet restore

# Build para iOS (Debug)
echo "🔨 Fazendo build para iOS (Debug)..."
dotnet build -f net8.0-ios -c Debug

if [ $? -eq 0 ]; then
    echo "✅ Build para iOS (Debug) concluído com sucesso!"
    
    # Build para iOS (Release)
    echo "🔨 Fazendo build para iOS (Release)..."
    dotnet build -f net8.0-ios -c Release
    
    if [ $? -eq 0 ]; then
        echo "✅ Build para iOS (Release) concluído com sucesso!"
        echo "🎉 Todos os builds foram concluídos!"
    else
        echo "❌ Erro no build Release"
        exit 1
    fi
else
    echo "❌ Erro no build Debug"
    exit 1
fi

echo ""
echo "📱 Para executar no simulador:"
echo "   dotnet build -f net8.0-ios -t:Run"
echo ""
echo "📱 Para criar IPA:"
echo "   dotnet publish -f net8.0-ios -c Release" 