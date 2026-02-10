#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
RESULTS_DIR="${SCRIPT_DIR}/results"
SCENARIOS_DIR="${SCRIPT_DIR}/scenarios"
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
RUN_DIR="${RESULTS_DIR}/${TIMESTAMP}"

mkdir -p "${RUN_DIR}"

GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m'

SCENARIOS=("smoke" "load" "stress" "spike" "soak" "websocket")
PASSED=0
FAILED=0
SKIPPED=0
RESULTS=()

echo ""
echo -e "${CYAN}=== Observa - Suite de Pruebas de Carga k6 ===${NC}"
echo -e "${CYAN}Inicio: $(date)${NC}"
echo -e "${CYAN}Resultados: ${RUN_DIR}${NC}"
echo ""

if [ -n "${K6_BASE_URL:-}" ]; then
    echo -e "${YELLOW}URL base: ${K6_BASE_URL}${NC}"
else
    echo -e "${YELLOW}URL base: http://localhost:5268 (default)${NC}"
fi
echo ""

if ! command -v k6 &> /dev/null; then
    echo -e "${RED}Error: k6 no esta instalado. Ejecuta: brew install k6${NC}"
    exit 1
fi

# Permitir seleccionar escenarios especificos via argumento
if [ $# -gt 0 ]; then
    SCENARIOS=("$@")
fi

for scenario in "${SCENARIOS[@]}"; do
    SCENARIO_FILE="${SCENARIOS_DIR}/${scenario}.js"

    if [ ! -f "${SCENARIO_FILE}" ]; then
        echo -e "${YELLOW}[SKIP] ${scenario}: archivo no encontrado${NC}"
        SKIPPED=$((SKIPPED + 1))
        RESULTS+=("SKIP:${scenario}")
        continue
    fi

    echo -e "${CYAN}--- Ejecutando: ${scenario} ---${NC}"
    echo ""

    JSON_OUT="${RUN_DIR}/${scenario}.json"
    SUMMARY_OUT="${RUN_DIR}/${scenario}_summary.json"
    LOG_OUT="${RUN_DIR}/${scenario}.log"

    if k6 run \
        --out "json=${JSON_OUT}" \
        --summary-export="${SUMMARY_OUT}" \
        "${SCENARIO_FILE}" 2>&1 | tee "${LOG_OUT}"; then
        echo ""
        echo -e "${GREEN}[PASS] ${scenario}${NC}"
        PASSED=$((PASSED + 1))
        RESULTS+=("PASS:${scenario}")
    else
        echo ""
        echo -e "${RED}[FAIL] ${scenario}${NC}"
        FAILED=$((FAILED + 1))
        RESULTS+=("FAIL:${scenario}")
    fi

    echo ""
done

echo ""
echo -e "${CYAN}=== Resumen de Ejecucion ===${NC}"
echo -e "${CYAN}Fin: $(date)${NC}"
echo ""

for result in "${RESULTS[@]}"; do
    STATUS="${result%%:*}"
    NAME="${result#*:}"
    case "${STATUS}" in
        PASS) echo -e "  ${GREEN}[PASS]${NC} ${NAME}" ;;
        FAIL) echo -e "  ${RED}[FAIL]${NC} ${NAME}" ;;
        SKIP) echo -e "  ${YELLOW}[SKIP]${NC} ${NAME}" ;;
    esac
done

echo ""
echo -e "Total: ${GREEN}${PASSED} passed${NC}, ${RED}${FAILED} failed${NC}, ${YELLOW}${SKIPPED} skipped${NC}"
echo -e "Reportes: ${RUN_DIR}"
echo ""

if [ "${FAILED}" -gt 0 ]; then
    exit 1
fi
