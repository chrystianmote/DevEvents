﻿using AutoMapper;
using DevEvents.API.Entities;
using DevEvents.API.Models;
using DevEvents.API.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevEvents.API.Controllers
{
    [Route("api/dev-events")]
    [ApiController]
    public class DevEventsController : ControllerBase
    {
        private readonly DevEventsDbContext _context;
        private readonly IMapper _mapper;


        public DevEventsController(DevEventsDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        /// <summary>
        /// Obter todos os eventos
        /// </summary>
        /// <returns>Coleção de Eventos</returns>
        /// <response code="200">Sucesso</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            //Traz todos os eventos onde não está deletado/cancelado 
            var devEvents = _context.DevEvents.Where(d => !d.IsDeleted).ToList();

            //Disponibiliza para o requisitante os dados dos eventos do viewmodel.
            var viewModel = _mapper.Map<List<DevEventViewModel>>(devEvents);

            return Ok(viewModel);
        }




        /// <summary>
        /// Obter um evento
        /// </summary>
        /// <param name="id">Identificador do Evento</param>
        /// <returns>Dados do Evento</returns>
        /// <response code="200">Sucesso</response>
        /// <response code="404">Não Encontrado</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(Guid id)
        {
            //Traz todos o evento pelo id
            var devEvent = _context.DevEvents
                .Include(de => de.Speakers)
                .SingleOrDefault(d => d.Id == id);

            var viewModel = _mapper.Map<DevEventViewModel>(devEvent);

            return devEvent == null ? NotFound() : Ok(viewModel);

        }


        /// <summary>
        /// Cadastrar um evento
        /// </summary>
        /// <remarks>
        /// {
        ///  "title": "Título do Evento",
        ///  "description": "Descrição do Evento",
        ///  "startDate": Data início do Evento ("2023-10-12T00:00:00.000Z"),
        ///  "endDate": Data final do Evento ("2023-10-13T00:00:00.000Z"),
        ///  "speakers": [],
        ///  "isDeleted": false (foi cancelado ou não)
        ///}
        /// </remarks>
        /// <param name="input">Dados do Evento</param>
        /// <returns>Objeto criado do Evento</returns>
        /// <response code="201">Sucesso</response>

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult Post(DevEventInputModel input)
        {
            //Coloca o modelo de entrada e converte pro modelo do Banco para cadastro
            var devEvent = _mapper.Map<DevEvent>(input);

            _context.DevEvents.Add(devEvent);
            _context.SaveChanges();

            //Converte o modelo de saída para exibicao ao requisitante
            var viewModel = _mapper.Map<DevEventViewModel>(devEvent);

            //retorna o modelo de saída após a conferência do objeto cadastrado.
            return CreatedAtAction(nameof(GetById), new { id = viewModel.Id }, viewModel);
        }


        /// <summary>
        /// Atualizar um Evento
        /// </summary>
        /// <remarks>
        /// {
        ///  "title": "Título do Evento",
        ///  "description": "Descrição do Evento",
        ///  "startDate": Data início do Evento ("2023-10-12T00:00:00.000Z"),
        ///  "endDate": Data final do Evento ("2023-10-13T00:00:00.000Z"),
        ///  "isDeleted": false (foi cancelado ou não)
        ///}
        /// </remarks>
        /// <param name="id">Identificador do Evento</param>
        /// <param name="input">Dados do Evento</param>
        /// <returns>Nenhum Conteúdo</returns>
        /// <response code="204">Sucesso</response>
        /// <response code="404">Não Encontrado</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Update(Guid id, DevEventInputModel input)
        {
            //Traz o evento pelo id
            var devEvent = _context.DevEvents.SingleOrDefault(d => d.Id == id);

            if (devEvent == null)
                return NotFound();

            devEvent.Update(input.Title, input.Description, input.StartDate, input.EndDate);

            _context.DevEvents.Update(devEvent);
            _context.SaveChanges();

            return NoContent();

        }


        /// <summary>
        /// Deletar um Evento
        /// </summary>
        /// <param name="id">Identificador do Evento</param>
        /// <returns>Nenhum Conteúdo</returns>
        /// <response code="204">Sucesso</response>
        /// <response code="404">Não Encontrado</response>

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(Guid id)
        {
            //Traz o evento pelo id
            var devEvent = _context.DevEvents.SingleOrDefault(d => d.Id == id);

            if (devEvent == null)
                return NotFound();

            devEvent.Delete();

            _context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Cadastrar um Palestrante de um Evento
        /// </summary>
        /// <remarks>
        ///    {
        ///      "name": "Nome do Palestrante",
        ///      "talkTitle": "Título da Palestra do Palestrante",
        ///      "talkDescription": "Assunto descritivo da Palestra",
        ///      "linkedInProfile": "Link do Perfil do LinkedIn"
        ///    }
        /// </remarks>
        /// <param name="id">Identificador do Evento</param>
        /// <param name="input">Dados do Palestrante</param>
        /// <returns>Nenhum Conteúdo</returns>
        /// <response code="204">Sucesso</response>
        /// <response code="404">Não Encontrado</response>

        [HttpPost("{id}/speakers")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult PostSpeaker(Guid id, DevEventSpeakerInputModel input)
        {
            //Coloca o modelo de entrada e converte pro modelo do Banco para cadastro
            var speaker = _mapper.Map<DevEventSpeaker>(input);

            speaker.DevEventId = id;
            var devEvent = _context.DevEvents.Any(d => d.Id == id);

            if (!devEvent)
                return NotFound();

            _context.DevEventSpeakers.Add(speaker);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
